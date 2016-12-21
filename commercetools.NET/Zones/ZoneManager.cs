﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

using commercetools.Common;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace commercetools.Zones
{
    /// <summary>
    /// Provides access to the functions in the Zones section of the API. 
    /// </summary>
    /// <see href="http://dev.commercetools.com/http-api-projects-zones.html"/>
    public class ZoneManager
    {
        #region Constants

        private const string ENDPOINT_PREFIX = "/zones";

        #endregion

        #region Member Variables

        private Client _client;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client">Client</param>
        public ZoneManager(Client client)
        {
            _client = client;
        }

        #endregion

        #region API Methods

        /// <summary>
        /// Gets a Zone by its ID.
        /// </summary>
        /// <param name="zoneId">Zone ID</param>
        /// <returns>Zone</returns>
        /// <see href="http://dev.commercetools.com/http-api-projects-zones.html#get-zone-by-id"/>
        public async Task<Response<Zone>> GetZoneByIdAsync(string zoneId)
        {
            if (string.IsNullOrWhiteSpace(zoneId))
            {
                throw new ArgumentException("zoneId is required");
            }

            string endpoint = string.Concat(ENDPOINT_PREFIX, "/", zoneId);
            return await _client.GetAsync<Zone>(endpoint);
        }

        /// <summary>
        /// Queries for Zones.
        /// </summary>
        /// <param name="where">Where</param>
        /// <param name="sort">Sort</param>
        /// <param name="limit">Limit</param>
        /// <param name="offset">Offset</param>
        /// <returns>ZoneQueryResult</returns>
        /// <see href="http://dev.commercetools.com/http-api-projects-zones.html#query-zones"/>
        public async Task<Response<ZoneQueryResult>> QueryZonesAsync(string where = null, string sort = null, int limit = -1, int offset = -1)
        {
            NameValueCollection values = new NameValueCollection();

            if (!string.IsNullOrWhiteSpace(where))
            {
                values.Add("where", where);
            }

            if (!string.IsNullOrWhiteSpace(sort))
            {
                values.Add("sort", sort);
            }

            if (limit > 0)
            {
                values.Add("limit", limit.ToString());
            }

            if (offset >= 0)
            {
                values.Add("offset", offset.ToString());
            }

            return await _client.GetAsync<ZoneQueryResult>(ENDPOINT_PREFIX, values);
        }

        /// <summary>
        /// Creates a new zone.
        /// </summary>
        /// <param name="zoneDraft">Zone Draft</param>
        /// <returns>Zone</returns>
        /// <see href="http://dev.commercetools.com/http-api-projects-zones.html#create-zone"/>
        public async Task<Response<Zone>> CreateZoneAsync(ZoneDraft zoneDraft)
        {
            if (string.IsNullOrWhiteSpace(zoneDraft.Name))
            {
                throw new ArgumentException("Name is required");
            }

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            string payload = JsonConvert.SerializeObject(zoneDraft, settings);
            return await _client.PostAsync<Zone>(ENDPOINT_PREFIX, payload);
        }

        /// <summary>
        /// Updates a zone.
        /// </summary>
        /// <param name="zone">Zone</param>
        /// <param name="action">The update action to be performed on the zone.</param>
        /// <returns>Zone</returns>
        /// <see href="http://dev.commercetools.com/http-api-projects-zones.html#update-zone"/>
        public async Task<Response<Zone>> UpdateZoneAsync(Zone zone, UpdateAction action)
        {
            return await UpdateZoneAsync(zone.Id, zone.Version, new List<UpdateAction> { action });
        }

        /// <summary>
        /// Updates a zone.
        /// </summary>
        /// <param name="zone">Zone</param>
        /// <param name="actions">The list of update actions to be performed on the zone.</param>
        /// <returns>Zone</returns>
        /// <see href="http://dev.commercetools.com/http-api-projects-zones.html#update-zone"/>
        public async Task<Response<Zone>> UpdateZoneAsync(Zone zone, List<UpdateAction> actions)
        {
            return await UpdateZoneAsync(zone.Id, zone.Version, actions);
        }

        /// <summary>
        /// Updates a zone.
        /// </summary>
        /// <param name="zoneId">ID of the zone</param>
        /// <param name="version">The expected version of the zone on which the changes should be applied.</param>
        /// <param name="actions">The list of update actions to be performed on the zone.</param>
        /// <returns>Zone</returns>
        /// <see href="http://dev.commercetools.com/http-api-projects-zones.html#update-zone"/>
        public async Task<Response<Zone>> UpdateZoneAsync(string zoneId, int version, List<UpdateAction> actions)
        {
            if (string.IsNullOrWhiteSpace(zoneId))
            {
                throw new ArgumentException("Zone ID is required");
            }

            if (version < 1)
            {
                throw new ArgumentException("Version is required");
            }

            if (actions == null || actions.Count < 1)
            {
                throw new ArgumentException("One or more update actions is required");
            }

            JObject data = JObject.FromObject(new
            {
                version = version,
                actions = new JArray(actions.ToArray())
            });

            string endpoint = string.Concat(ENDPOINT_PREFIX, "/", zoneId);
            return await _client.PostAsync<Zone>(endpoint, data.ToString());
        }

        /// <summary>
        /// Deletes a zone.
        /// </summary>
        /// <param name="zone">Zone</param>
        /// <see href="http://dev.commercetools.com/http-api-projects-zones.html#delete-zone"/>
        public async Task<Response<JObject>> DeleteZoneAsync(Zone zone)
        {
            return await DeleteZoneAsync(zone.Id, zone.Version);
        }

        /// <summary>
        /// Deletes a zone.
        /// </summary>
        /// <param name="zoneId">Zone ID</param>
        /// <param name="version">Zone version</param>
        /// <see href="http://dev.commercetools.com/http-api-projects-zones.html#delete-zone"/>
        public async Task<Response<JObject>> DeleteZoneAsync(string zoneId, int version)
        {
            if (string.IsNullOrWhiteSpace(zoneId))
            {
                throw new ArgumentException("Zone ID is required");
            }

            if (version < 1)
            {
                throw new ArgumentException("Version is required");
            }

            var values = new NameValueCollection
            {
                { "version", version.ToString() }
            };

            string endpoint = string.Concat(ENDPOINT_PREFIX, "/", zoneId);
            return await _client.DeleteAsync<JObject>(endpoint, values);
        }

        #endregion
    }
}
