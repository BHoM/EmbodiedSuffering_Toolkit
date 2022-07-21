/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.EmbodiedSuffering.Elements;
using BH.Engine.Library;

namespace BH.Engine.EmbodiedSuffering
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Returns the Suffering Index based on Material Imports data.")]
        [Input("numberOfEnslavedPeople", "A list of values from the GlobalSlaveryIndex dataset for the country/countries you wish to evaluate.")]
        [Input("materialImportSource", "The MaterialImportSources dataset or object used to calculate the resultant index value.")]
        [Input("slaveryData", "The Slavery data used to calculate the resultant index value.")]
        [Input("name", "The name of the SufferingIndex.")]
        [Output("sufferingIndex", "The SufferingIndex per import location and slavery data.")]
        public static List<double> SufferingIndex(List<double> numberOfEnslavedPeople, MaterialImportSources materialImportData, Slavery slaveryData, string name)
        {
            // Issues that need to be resolved before merge: 
            // slaveryData is not being used and is currently implemented prior to method call by the user (manual)
            // name is not being used
            // What is the desired output of this method. A list of doubles, a single double, a list of objects?

            List<double> sufferingIndex = new List<double>();
            List<double> importRatios = materialImportData.ImportRatios;

            // null check and return 0 for numPeople
            for (int i = 0; i < numberOfEnslavedPeople.Count; i++)
            {
                if (numberOfEnslavedPeople[i] == double.NaN)
                {
                    numberOfEnslavedPeople[i] = 0;
                }
            }

            // check if the lists are the same length
            if (numberOfEnslavedPeople.Count != importRatios.Count)
            {
                BH.Engine.Base.Compute.RecordError("The provided list's lengths to do correspond as expected. Results cannot be computed.");
                return sufferingIndex; 
            }

            // multiply values
            for (int i = 0; i < numberOfEnslavedPeople.Count; i++)
            {
                sufferingIndex.Add(numberOfEnslavedPeople[i] * materialImportData.ImportRatios[i]);
            }

            List<string> exportCountries = (List<string>)materialImportData.ExportCountries.ToList().Select(x => x.ToString());

            return sufferingIndex;
        }
        /***************************************************/

    }
}