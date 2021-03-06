//Copyright (c) 2004-2007
//by Engage Software ( http://www.engagesoftware.com )

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
//DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Data;
using DotNetNuke.Common.Lists;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Common.Utilities;
using Engage.Dnn.Locator.Data;
using Microsoft.ApplicationBlocks.Data;

namespace Engage.Dnn.Locator
{
    using System.Globalization;

    public class LocationType
    {
        public const string CacheKey = "AttributeDefinitions{0}";
        public const int CacheTimeOut = 20;

        #region Properties

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _locationId;
        public int LocationId
        {
            [System.Diagnostics.DebuggerStepThroughAttribute()]
            get { return _locationId; }
            [System.Diagnostics.DebuggerStepThroughAttribute()]
            set { _locationId = value; }
        }

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _locationTypeName;        
        public string LocationTypeName
        {
            [System.Diagnostics.DebuggerStepThroughAttribute()]
            get { return _locationTypeName; }
            [System.Diagnostics.DebuggerStepThroughAttribute()]
            set { _locationTypeName = value; }
        }

        #endregion

        #region Custom Attributes

        private static AttributeDefinitionCollection FillCollection(IDataReader dr)
        {
            List<AttributeDefinition> definitions = CBO.FillCollection<AttributeDefinition>(dr);
            AttributeDefinitionCollection definitionsCollection = new AttributeDefinitionCollection();
            foreach (AttributeDefinition definition in definitions)
            {
                //Clear the Is Dirty Flag
                definition.ClearIsDirty();

                //Initialise the Visibility
                object setting = UserModuleBase.GetSetting(definition.PortalId, "LocationType_DefaultVisibility");
                if ((setting != null))
                {
                    definition.Visibility = (DotNetNuke.Entities.Users.UserVisibilityMode)setting;
                }

                //Add to collection
                definitionsCollection.Add(definition);
            }
            return definitionsCollection;
        }

        private static AttributeDefinition FillAttributeDefinitionInfo(IDataReader dr)
        {
            AttributeDefinition definition = null;

            try
            {
                definition = FillAttributeDefinitionInfo(dr, false);
            }
            catch
            {
            }
            finally
            {
                if ((dr != null))
                {
                    dr.Close();
                }
            }

            return definition;
        }

        private static AttributeDefinition FillAttributeDefinitionInfo(IDataReader dr, bool checkForOpenDataReader)
        {
            AttributeDefinition definition = null;

            // read datareader
            bool canContinue = true;
            if (checkForOpenDataReader)
            {
                canContinue = false;
                if (dr.Read())
                {
                    canContinue = true;
                }
            }

            if (canContinue)
            {
                definition = new AttributeDefinition();
                definition.AttributeDefinitionId = Convert.ToInt32(Null.SetNull(dr["AttributeDefinitionId"], definition.AttributeDefinitionId), CultureInfo.InvariantCulture);
                definition.PortalId = Convert.ToInt32(Null.SetNull(dr["PortalId"], definition.PortalId), CultureInfo.InvariantCulture);
                definition.LocationTypeId = Convert.ToInt32(Null.SetNull(dr["LocationTypeId"], definition.LocationTypeId), CultureInfo.InvariantCulture);
                definition.DataType = Convert.ToInt32(Null.SetNull(dr["DataType"], definition.DataType), CultureInfo.InvariantCulture);
                definition.DefaultValue = Convert.ToString(Null.SetNull(dr["DefaultValue"], definition.DefaultValue), CultureInfo.InvariantCulture);
                ////definition.AttributeCategory = Convert.ToString(Null.SetNull(dr["AttributeCategory"], definition.AttributeCategory), CultureInfo.InvariantCulture);
                definition.AttributeName = Convert.ToString(Null.SetNull(dr["AttributeName"], definition.AttributeName), CultureInfo.InvariantCulture);
                definition.Length = Convert.ToInt32(Null.SetNull(dr["Length"], definition.Length), CultureInfo.InvariantCulture);
                definition.Required = Convert.ToBoolean(Null.SetNull(dr["Required"], definition.Required), CultureInfo.InvariantCulture);
                definition.ValidationExpression = Convert.ToString(Null.SetNull(dr["ValidationExpression"], definition.ValidationExpression), CultureInfo.InvariantCulture);
                definition.ViewOrder = Convert.ToInt32(Null.SetNull(dr["ViewOrder"], definition.ViewOrder), CultureInfo.InvariantCulture);
                definition.Visible = Convert.ToBoolean(Null.SetNull(dr["Visible"], definition.Visible), CultureInfo.InvariantCulture);
            }

            return definition;
        }

        private static List<AttributeDefinition> FillAttributeDefinitionInfoCollection(IDataReader dr)
        {
            List<AttributeDefinition> arr = new List<AttributeDefinition>();
            try
            {
                AttributeDefinition obj;
                while (dr.Read())
                {
                    // fill business object
                    obj = FillAttributeDefinitionInfo(dr, false);
                    // add to collection
                    arr.Add(obj);
                }
            }
            catch (Exception exc)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(exc);
            }
            finally
            {
                // close datareader
                if ((dr != null))
                {
                    dr.Close();
                }
            }

            return arr;
        }

        public static List<AttributeDefinition> GetAttributeDefinitions(int locationTypeId)
        {
            List<AttributeDefinition> definitions = FillAttributeDefinitionInfoCollection(DataProvider.Instance().GetAttributeDefinitionsById(locationTypeId));
            return definitions;
        }

        #endregion

        #region Attribute Definition Methods

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Adds a Property Defintion to the Data Store
        /// </summary>
        /// <param name="definition">An LocationTypeAttributeDefinition object</param>
        /// <returns>The Id of the definition (or if negative the errorcode of the error)</returns>
        /// -----------------------------------------------------------------------------
        public static int AddAttributeDefinition(AttributeDefinition definition)
        {
            if (definition.Required)
            {
                definition.Visible = true;
            }

            int id = DataProvider.Instance().AddAttributeDefinition(definition.PortalId, definition.LocationTypeId, definition.DataType, definition.DefaultValue, definition.AttributeName, definition.Required, definition.ValidationExpression, definition.ViewOrder, definition.Visible, definition.Length);

            ClearCache(definition.LocationTypeId);

            return id;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Clears the LocationType Definitions Cache
        /// </summary>
        /// -----------------------------------------------------------------------------
        public static void ClearCache(int locationTypeId)
        {
            string key = String.Format(CultureInfo.InvariantCulture, CacheKey, locationTypeId);
            DataCache.RemoveCache(key);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Deletes a Property Defintion from the Data Store
        /// </summary>
        /// <param name="definition">The LocationTypeAttributeDefinition object to delete</param>
        /// -----------------------------------------------------------------------------
        public static void DeleteAttributeDefinition(AttributeDefinition definition)
        {
            DataProvider.Instance().DeleteAttributeDefinition(definition.AttributeDefinitionId);
            ClearCache(definition.LocationTypeId);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets a Property Defintion from the Data Store by id
        /// </summary>
        /// <param name="definitionId">The id of the LocationTypeAttributeDefinition object to retrieve</param>
        /// <returns>The LocationTypeAttributeDefinition object</returns>
        /// -----------------------------------------------------------------------------
        public static AttributeDefinition GetAttributeDefinition(int definitionId, int locationTypeId)
        {
            AttributeDefinition definition = null;
            bool bFound = Null.NullBoolean;

            foreach (AttributeDefinition singleDefinition in GetAttributeDefinitions(locationTypeId))
            {
                if (singleDefinition.AttributeDefinitionId == definitionId)
                {
                    bFound = true;
                    definition = singleDefinition;
                    break; // TODO: might not be correct. Was : Exit For
                }
            }

            if (!bFound)
            {
                //Try Database
                definition = FillAttributeDefinitionInfo(DataProvider.Instance().GetAttributeDefinition(definitionId));
            }

            return definition;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets a Property Defintion from the Data Store by name
        /// </summary>
        /// <param name="portalId">The id of the Portal</param>
        /// <param name="name">The name of the LocationTypeAttributeDefinition object to retrieve</param>
        /// <returns>The LocationTypeAttributeDefinition object</returns>
        /// -----------------------------------------------------------------------------
        public static AttributeDefinition GetAttributeDefinitionByName(int locationTypeId, string name)
        {
            AttributeDefinition definition = null;
            bool bFound = Null.NullBoolean;

            foreach (AttributeDefinition singleDefinition in GetAttributeDefinitions(locationTypeId))
            {
                if (singleDefinition.AttributeName == name)
                {
                    bFound = true;
                    definition = singleDefinition;
                    break; // TODO: might not be correct. Was : Exit For
                }
            }

            if (!bFound)
            {
                //Try Database
                definition = FillAttributeDefinitionInfo(DataProvider.Instance().GetAttributeDefinitionByName(locationTypeId, name));
            }

            return definition;
        }


        public static AttributeDefinitionCollection GetAttributeDefinitionsById(int portalId)
        {
            return GetAttributeDefinitionsById(portalId, true);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets a collection of Property Defintions from the Data Store by portal
        /// </summary>
        /// <returns>A LocationTypeAttributeDefinitionCollection object</returns>
        /// -----------------------------------------------------------------------------
        public static AttributeDefinitionCollection GetAttributeDefinitionsById(int locationTypeId, bool clone)
        {
            string key = string.Format(CultureInfo.InvariantCulture, CacheKey, locationTypeId);

            // Try fetching the List from the Cache
            AttributeDefinitionCollection attributes = (AttributeDefinitionCollection)DataCache.GetCache(key);

            if (attributes == null)
            {
                attributes = new AttributeDefinitionCollection();
                int timeOut = CacheTimeOut * Convert.ToInt32(DotNetNuke.Common.Globals.PerformanceSetting, CultureInfo.InvariantCulture);

                foreach (AttributeDefinition definition in GetAttributeDefinitions(locationTypeId))
                {
                    if (clone)
                    {
                        attributes.Add(definition.Clone());
                    }
                    else
                    {
                        attributes.Add(definition);
                    }
                }

                if (timeOut > 0)
                {
                    DataCache.SetCache(key, attributes, TimeSpan.FromMinutes(timeOut));
                }
            }

            return attributes;
        }

        public static void UpdateAttributeDefinition(AttributeDefinition definition)
        {
            if (definition.Required)
            {
                definition.Visible = true;
            }
            DataProvider.Instance().UpdateAttributeDefinition(definition.AttributeDefinitionId, definition.DataType, definition.DefaultValue, definition.AttributeName, definition.Required, definition.ValidationExpression, definition.ViewOrder, definition.Visible, definition.Length);

            ClearCache(definition.LocationTypeId);
        }

        #endregion

        public static DataTable GetLocationTypes()
        {
            return DataProvider.Instance().GetLocationTypes();
        }

        public static bool GetLocationTypeInUse(string location)
        {
            bool inuse = false;
            int count;

            count = DataProvider.Instance().GetLocationTypeCount(location);
            if (count > 0) inuse = true;

            return inuse;
        }

        public static string GetLocationTypeName(int id)
        {
            DataTable dt = DataProvider.Instance().GetLocationTypeName(id);
            string name = string.Empty;
            if (dt.Rows.Count == 1)
            {
                name = dt.Rows[0]["LocationTypeName"].ToString();
            }

            return name;
        }



    }
}