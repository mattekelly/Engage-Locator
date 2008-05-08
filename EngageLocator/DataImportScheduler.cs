using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading;
using DotNetNuke.Common.Lists;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using Engage.Dnn.Locator.Data;
using LumenWorks.Framework.IO.Csv;

namespace Engage.Dnn.Locator
{
    public class DataImportScheduler
    {
        private const string ImportFolderName = "Location Import";
        private const string WorkingFolderName = "Location Import/Working/";
        private const string CompletedFolderName = "Location Import/Completed/";
        private const string ErrorFolderName = "Location Import/Error/";
        private int _portalId;

        public DataImportScheduler()
        {
            ThreadPool.QueueUserWorkItem(DoWork);
        }

        public void DoWork(Object threadContext)
        {
            if (HasFilesToImport)
            {
                while (HasFilesToImport)
                {
                    DataTable files = Location.GetFilesToImport();

                    try
                    {
                        foreach (DataRow file in files.Rows)
                        {
                            _portalId = Convert.ToInt32(file["PortalId"].ToString());

                            FolderInfo fiLocation = FileSystemUtils.GetFolder(_portalId, ImportFolderName);
                            ArrayList filesLocation = FileSystemUtils.GetFilesByFolder(_portalId, fiLocation.FolderID);

                            FolderInfo fiLocationWorking = FileSystemUtils.GetFolder(_portalId, WorkingFolderName);
                            ArrayList filesLocationWorking = FileSystemUtils.GetFilesByFolder(_portalId, fiLocationWorking.FolderID);

                            int index = DataProvider.Instance().GetLastImportIndex();

                            if (filesLocation.Count > 0)
                            {
                                foreach (DotNetNuke.Services.FileSystem.FileInfo fileInfo in filesLocationWorking)
                                {
                                    File.Delete(fileInfo.PhysicalPath);
                                }
                                StageData(filesLocation, fiLocationWorking);
                                CollectData(fiLocationWorking, index, _portalId);
                            }
                            else if (filesLocationWorking.Count > 0)
                            {
                                CollectData(fiLocationWorking, index, _portalId);
                            }
                            int id = Convert.ToInt32(file["FileId"].ToString());
                            UpdateImportedLocationRow(id);
                        }

                    }
                    catch (Exception ex)
                    {
                        Exceptions.LogException(ex);
                        FileMove(false);
                    }
                }
            }
        }

        public void StageData(ArrayList files, FolderInfo folderInfo)
        {
            FileController fc = new FileController();
            if (folderInfo != null && files != null)
            {
                foreach (DotNetNuke.Services.FileSystem.FileInfo fileInfo in files)
                {
                    File.Move(fileInfo.PhysicalPath, folderInfo.PhysicalPath + fileInfo.FileName);
                    fc.UpdateFile(fileInfo.FileId, fileInfo.FileName, fileInfo.Extension.Substring(1), fileInfo.Size, Null.NullInteger, Null.NullInteger, fileInfo.ContentType, folderInfo.PhysicalPath, folderInfo.FolderID);
                }
            }
        }


        public void CollectData(FolderInfo folderInfo, int startIndex, int portalId)
        {
            ArrayList filesWorking = FileSystemUtils.GetFilesByFolder(portalId, folderInfo.FolderID);

            if (filesWorking != null)
            {
                foreach (DotNetNuke.Services.FileSystem.FileInfo fileInfo in filesWorking)
                {
                    ParseCSV(fileInfo, startIndex);
                }
            }
            ImportData(_portalId);
        }

        public void ParseCSV(DotNetNuke.Services.FileSystem.FileInfo fileInfo, int startIndex)
        {
            using (CsvReader reader = new CsvReader(new StreamReader(fileInfo.PhysicalPath), true))
            {
                int tabModuleId = DataProvider.Instance().GetTabModuleIdByFileId(fileInfo.FileId);
                int lineNumber = 0;

                if (startIndex == 0)
                {
                    DataProvider.Instance().ClearTempLocations();
                }
                else
                {
                    lineNumber = startIndex;
                    reader.MoveTo(startIndex - 1);
                }

                try
                {
                    while (reader.ReadNextRecord())
                    {
                        lineNumber++;
                        Location location = new Location();

                        string address1, address2, state, country;
                        location.LocationId = Convert.ToInt32(GetValue(reader, "UNIQUE_KEY"));
                        location.ExternalIdentifier = GetValue(reader, "EXTERNAL_IDENTIFIER");
                        location.Name = GetValue(reader, "LOCATION_NAME");
                        address1 = GetValue(reader, "ADDRESS1");
                        address2 = GetValue(reader, "ADDRESS2");
                        location.City = GetValue(reader, "CITY");
                        state = GetValue(reader, "STATE");
                        location.PostalCode = GetValue(reader, "ZIP");
                        location.Phone = GetValue(reader, "PHONE_NUMBER");
                        location.LocationDetails = GetValue(reader, "LOCATION_DETAILS");
                        location.Website = GetValue(reader, "WEBSITE");
                        location.PortalId = _portalId;
                        string locationType = GetValue(reader, "TYPE_OF_LOCATION");
                        if (locationType == string.Empty)
                            locationType = "Default";
                        country = GetValue(reader, "COUNTRY");
                        DataTable dt = DataProvider.Instance().GetLocationTypes();
                        int locationTypeId = -1;

                        foreach (DataRow dr in dt.Rows)
                        {
                            if (Convert.ToString(dr["LocationTypeName"]) == locationType)
                            {

                                locationTypeId = Convert.ToInt32(dr["LocationTypeID"]);
                                location.LocationTypeId = locationTypeId;
                            }
                        }

                        if (locationTypeId == -1)
                        {
                            locationTypeId = DataProvider.Instance().InsertLocationType(locationType);
                            location.LocationTypeId = locationTypeId;
                        }

                        location.RegionId = ResolveState(state);
                        if (location.RegionId < 1)
                        {
                            location.RegionId = Convert.ToInt32(null);
                        }

                        location.CountryId = ResolveCountry(country);
                        if (location.CountryId < 1)
                        {
                            ModuleController objModules = new ModuleController();
                            location.CountryId = Convert.ToInt32(objModules.GetTabModuleSettings(tabModuleId)["DefaultCountry"].ToString());
                        }
                        location.CsvLineNumber = lineNumber;

                        Nullable<double> latitude;
                        Nullable<double> longitude;
                        string error;
                        location.Address = GetGeoCoordinates(tabModuleId, address1, address2, location.City, state, country, location.PostalCode, out latitude, out longitude, out error);

                        location.Latitude = Convert.ToDouble(latitude);
                        location.Longitude = Convert.ToDouble(longitude);
                        location.Approved = true;

                        if ((latitude == 0 && longitude == 0))
                        {
                            try
                            {
                                location.SaveTemp(false);
                            }
                            catch (SqlException ex)
                            {
                                Exceptions.LogException(ex);
                            }
                        }
                        else
                        {
                            try
                            {
                                location.SaveTemp(true);
                            }
                            catch (SqlException exc)
                            {
                                Exceptions.LogException(exc);
                            }
                        }
                    }
                }
                catch (ArgumentException exc)
                {
                    Exceptions.LogException(exc);
                    FileMove(false);
                }

                if (lineNumber == 0)
                {
                    FileMove(false);
                }
            }
        }

        public static string GetGeoCoordinates(int tabModuleId, string address1, string address2, string city, string state, string country, string zip, out double? latitude, out double? longitude, out string error)
        {
            string address;

            StringBuilder location = GetLocation(address1, string.Empty, ref city, state, country, out address);

            DataTable existingLatitudeLongitude = DataProvider.Instance().GetLatitudeLongitude(address, city);
            error = "";
            if (existingLatitudeLongitude.Rows.Count == 0)
            {
                GetGeoCodeResults(tabModuleId, city, state, zip, address, out latitude, out longitude, out error, location.ToString());
            }
            else
            {
                if (existingLatitudeLongitude.Rows[0]["Latitude"].ToString() == "")
                {
                    latitude = null;
                }
                else
                {
                    latitude = Convert.ToDouble(existingLatitudeLongitude.Rows[0]["Latitude"]);
                }
                if (existingLatitudeLongitude.Rows[0]["Longitude"].ToString() == "")
                {
                    longitude = null;
                }
                else
                {
                    longitude = Convert.ToDouble(existingLatitudeLongitude.Rows[0]["Longitude"]);
                }
            }
            location = GetLocation(address1, address2, ref city, state, country, out address);
            return address;
        }

        private static StringBuilder GetLocation(string address1, string address2, ref string city, string state, string country, out string address)
        {
            StringBuilder location = new StringBuilder(address1);
            if (!String.IsNullOrEmpty(address2))
            {
                location.Append(", " + address2);
            }

            address = location.ToString();

            if (!String.IsNullOrEmpty(city))
            {
                location.Append(", " + city);
            }
            else
            {
                city = "No City";
            }
            if (!String.IsNullOrEmpty(state))
            {
                location.Append(", " + state);
            }
            if (!String.IsNullOrEmpty(country))
            {
                location.Append(", " + country);
            }
            return location;
        }

        private static void GetGeoCodeResults(int tabModuleId, string city, string state, string zip, string address, out double? latitude, out double? longitude, out string error, string location)
        {
            YahooGeocodeResult yahoo = new YahooGeocodeResult();
            GoogleGeocodeResult google = new GoogleGeocodeResult();
            yahoo.accuracyCode = YahooAccuracyCode.Unknown;

            string apiKey = String.Empty;
            ModuleController mc = new ModuleController();
            Hashtable tabModuleSettings = mc.GetTabModuleSettings(tabModuleId);

            string displayProvider = Convert.ToString(tabModuleSettings["DisplayProvider"]);
            ReadOnlyCollection<MapProviderType> mpType = MapProviderType.GetMapProviderTypes();
            foreach (MapProviderType type in mpType)
            {
                if (type.ClassName.Contains(displayProvider))
                {
                    apiKey = type.GetApiKey(tabModuleSettings);
                    break;
                }
            }

            if (displayProvider == "Yahoo")
            {
                yahoo = SearchUtility.SearchYahoo(location, address, city, state, zip, apiKey);
                latitude = yahoo.latitude;
                longitude = yahoo.longitude;
                error = yahoo.statusCode.ToString();
            }
            else if (displayProvider == "Google")
            {
                google = SearchUtility.SearchGoogle(location, apiKey);
                latitude = google.latitude;
                longitude = google.longitude;
                error = google.statusCode.ToString();
            }
            else
            {
                latitude = null;
                longitude = null;
                error = google.statusCode + " - " + yahoo.statusCode;
            }
        }

        private void ImportData(int portalId)
        {
            string emailFrom = DotNetNuke.Entities.Host.HostSettings.GetHostSetting("HostEmail");
            StringBuilder emailBody = new StringBuilder();
            emailBody.AppendFormat("Your locations file has been imported.<br />");
            StringBuilder unsuccessfulLocations = new StringBuilder();

            DataTable locations = DataProvider.Instance().GetImportedLocationStatistics(portalId);
            int successfulCount = 0;
            int unsuccessfulCount = 0;
            foreach (DataRow row in locations.Rows)
            {
                if (row["Successful"].ToString() == "True")
                {
                    successfulCount++;
                }
                else
                {
                    unsuccessfulCount++;
                    unsuccessfulLocations.AppendFormat(row["LocationId"] + ", ");
                    unsuccessfulLocations.AppendFormat(row["Name"] + "<br />");
                }
            }
            if (unsuccessfulCount > 0)
            {
                emailBody.AppendFormat("There were " + successfulCount + " out of " + locations.Rows.Count + " locations imported successfully. <br />");
                emailBody.AppendFormat("The following locations (UNIQUE_KEY, LOCATION_NAME) from the csv file were not imported: <br /><br />");
                emailBody.AppendFormat(unsuccessfulLocations + "<br />");
            }
            else
            {
                emailBody.AppendFormat("All " + successfulCount + " out of " + locations.Rows.Count + " locations were imported successfully.",
                    Environment.NewLine);
            }

            FolderInfo fi = FileSystemUtils.GetFolder(portalId, WorkingFolderName);
            ArrayList files = FileSystemUtils.GetFilesByFolder(portalId, fi.FolderID);

            //how should this be handled correctly
            DataProvider.Instance().ClearLocations(portalId);
            DataProvider.Instance().CopyData();
            DataProvider.Instance().ClearTempLocations();

            string emailSubject = "Engage: Locator Import Succeeded";

            foreach (DotNetNuke.Services.FileSystem.FileInfo fileInfo in files)
            {
                DataTable dt = DataProvider.Instance().GetEmailByFileId(fileInfo.FileId);
                if (dt.Rows.Count > 0)
                {
                    string email = Convert.ToString(dt.DefaultView[0].Row[3]);
                    if (email == "")
                        email = "local@localhost.com";
                    if (emailFrom == "")
                        emailFrom = "local@localhost.com";
                    DotNetNuke.Services.Mail.Mail.SendMail(emailFrom, email, "", emailSubject, emailBody.ToString(), "", "html", "", "", "", "");
                }
            }
            FileMove(true);
        }

        private static string GetValue(CsvReader reader, string colName)
        {
            if (!String.IsNullOrEmpty(colName))
            {
                return reader[colName];
            }
            return string.Empty;
        }

        public bool FileMove(bool success)
        {
            try
            {
                FolderInfo fiLocationCompleted = FileSystemUtils.GetFolder(_portalId, CompletedFolderName);

                if (fiLocationCompleted != null)
                {
                    ArrayList filesLocationCompleted = FileSystemUtils.GetFilesByFolder(_portalId, fiLocationCompleted.FolderID);
                    foreach (DotNetNuke.Services.FileSystem.FileInfo fileInfo in filesLocationCompleted)
                    {
                        File.Delete(fileInfo.PhysicalPath);
                    }

                    FolderInfo fiLocationError = FileSystemUtils.GetFolder(_portalId, ErrorFolderName);
                    ArrayList filesLocationError = FileSystemUtils.GetFilesByFolder(_portalId, fiLocationError.FolderID);
                    foreach (DotNetNuke.Services.FileSystem.FileInfo fileInfo in filesLocationError)
                    {
                        File.Delete(fileInfo.PhysicalPath);
                    }

                    FolderInfo fiLocationWorking = FileSystemUtils.GetFolder(_portalId, WorkingFolderName);
                    ArrayList filesLocationWorking = FileSystemUtils.GetFilesByFolder(_portalId, fiLocationWorking.FolderID);
                    foreach (DotNetNuke.Services.FileSystem.FileInfo fileInfo in filesLocationWorking)
                    {
                        if (success)
                        {
                            File.Move(fileInfo.PhysicalPath, fiLocationCompleted.PhysicalPath + fileInfo.FileId + fileInfo.FileName);
                        }
                        else
                        {
                            string fileToMove = fiLocationError.PhysicalPath + fileInfo.FileName;
                            if (File.Exists(fileToMove))
                            {
                                File.Move(fileInfo.PhysicalPath, fileToMove);
                            }
                        }
                    }
                }
            }
            catch (IOException exc)
            {
                Exceptions.LogException(exc);
                return false;
            }
            return true;
        }

        private static int ResolveState(string state)
        {
            int id = -1;
            ListController controller = new ListController();
            ListEntryInfoCollection regions = controller.GetListEntryInfoCollection("Region");
            foreach (ListEntryInfo region in regions)
            {
                if (region.Text == state || region.Value == state)
                {
                    id = region.EntryID;
                }
            }

            return id;
        }

        private static int ResolveCountry(string country)
        {
            int id = -1;
            ListController controller = new ListController();
            ListEntryInfoCollection info = controller.GetListEntryInfoCollection("Country"); // controller.GetListEntryInfo("Country", country);
            foreach (ListEntryInfo entry in info)
            {
                if (entry.Text == country)
                {
                    id = entry.EntryID;
                }
            }

            return id;
        }

        private static void UpdateImportedLocationRow(int fileId)
        {
            Location.UpdateImportedLocationRow(fileId);
        }

        private static bool HasFilesToImport
        {
            get
            {
                DataTable dt = Location.GetFilesToImport();
                if (dt.Rows.Count > 0)
                    return true;
                else return false;
            }
        }
    }
}