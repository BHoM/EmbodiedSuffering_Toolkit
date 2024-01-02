/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.EmbodiedSuffering.Elements;

namespace BH.Engine.EmbodiedSuffering
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the Data for VictimsOfModernSlavery by country based on data from Global Slavery Index.")]
        [Output("labourRisk", "The LabourExploitationRisk with freedom of association values based on data from GSI.")]
        public static List<LabourExploitationRisk> GlobalSlaveryIndex()
        {
            string datasetPath = "EmbodiedSuffering\\LabourExploitationRisk\\2018GlobalSlaveryIndex";
            return Library.Query.Library(datasetPath).OfType<LabourExploitationRisk>().ToList();
        }

        /***************************************************/

        [Description("Gets the Data for VictimsOfModernSlavery by country based on data from Global Slavery Index as a dictionary with keys as countries and values as the VictimsOfModernSlavery.")]
        [Output("slaveryVictims", "The VictimsOfModernSlavery with per country from Global Slavery Index.")]
        public static Dictionary<Country, double> GlobalSlaveryIndexDictionary()
        {
            if (m_globalSlaveryIndex != null)
            {
                DuplicateCountriesWarning();
                return m_globalSlaveryIndex;
            }

            List<LabourExploitationRisk> labourRisk = GlobalSlaveryIndex();

            m_duplicateGSICountries = labourRisk.GroupBy(x => x.Country).Where(x => x.Count() > 1).Select(x => x.Key).ToList();

            DuplicateCountriesWarning();
            m_globalSlaveryIndex = labourRisk.GroupBy(x => x.Country).ToDictionary(x => x.Key, x => x.Average(y => y.VictimsOfModernSlavery));
            return m_globalSlaveryIndex;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void DuplicateCountriesWarning()
        {
            if (m_duplicateGSICountries.Count > 0)
                BH.Engine.Base.Compute.RecordWarning($"Multiple records in GlobalSlaveryIndex for the following countries {string.Join(", ", m_duplicateGSICountries)}. Average values in dataset for those countries returned.");
        }

        /***************************************************/
        /**** Private fields                            ****/
        /***************************************************/

        private static Dictionary<Country, double> m_globalSlaveryIndex = null;
        private static List<Country> m_duplicateGSICountries;

        /***************************************************/
    }
}


