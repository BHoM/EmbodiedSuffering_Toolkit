﻿/*
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
        [Input("acceptableThreshold", "The acceptable value of Number of Enslaved People to be considered in the calculation. Values found in the NumberOfEnslavedPeople input above this threshold will not be considered.")]
        [MultiOutput(0, "sufferingIndex", "The SufferingIndex per import location and slavery data.")]
        [MultiOutput(1, "culledCountries", "Countries removed from the calculation due to insufficient data.")]
        [MultiOutput(2, "invalidCountries", "Countries that had no data for NumberOfEnslavedPeople.")]
        public static Output<double, List<string>, List<string>> SufferingIndex(List<double> numberOfEnslavedPeople, MaterialImportSources materialImportSources, double acceptableThreshold)
        {
            double sufferingIndex = 0;
            List<double> importRatios = materialImportSources.ImportRatios.ToList();
            List<List<Country>> exportCountries = new List<List<Country>>();
            List<string> culledCountries = new List<string>();
            List<string> invalidCountries = new List<string>();

            // List length check: Fail the calculation if the lists are of unequal lengths
            if (numberOfEnslavedPeople.Count != importRatios.Count)
            {
                int numCount = numberOfEnslavedPeople.Count;
                int ratiosCount = importRatios.Count;
                BH.Engine.Base.Compute.RecordError("The provided list's lengths to do correspond as expected." + $"\nNumberOfEnslavedPeople count = {numCount}" + $"\nImportRatios count = {ratiosCount}. \nPlease update your data and try again.");
            } else
            {
                // Remove after debugging is complete
                BH.Engine.Base.Compute.RecordNote("Lists are the same length, proceeding to threshold check.");
            }

            // Add countries to ImportCountries[]
            for (int i = 0; i < materialImportSources.ImportRatios.Count; i++)
            {
                exportCountries.Add(materialImportSources.ExportCountries);
            }

            // Pass a list of string import countries
            // This is used in the threshold check
            List<string> importCountriesAsStrings = exportCountries.ConvertAll(x => x.ToString());

            // Threshold check: Toss anything above the input threshold into badCountries[]
            for (int i = 0; i < numberOfEnslavedPeople.Count; i++)
            {
                if (numberOfEnslavedPeople[i] > acceptableThreshold)
                {
                    culledCountries.Add(importCountriesAsStrings[i]);
                    numberOfEnslavedPeople.Remove(numberOfEnslavedPeople[i]);
                    BH.Engine.Base.Compute.RecordWarning("Some countries were removed from your calculation due to exceeding the acceptableThreshold requirement. \nPlease review them from the CulledCountries output. \nThe calculation will now proceed with all remaining countries.");
                }
            }

            // Data validitiy check: Toss anything with 0 or NaN into invalidCountries[]
            for (int i = 0; i < numberOfEnslavedPeople.Count; i++)
            {
                if (numberOfEnslavedPeople[i] == double.NaN)
                {
                    numberOfEnslavedPeople[i] = 0;
                    invalidCountries.Add(importCountriesAsStrings[i]);
                    BH.Engine.Base.Compute.RecordWarning("Some country's number of enslaved people was defaulted to zero due to missing or invalid data. \nPlease review them from the invalidCountries output. \nThe calculation will now proceed with all remaining countries.");
                }
            }

            // Calculation: Multiply the lists of number of people by the import ratios
            for (int i = 0; i < numberOfEnslavedPeople.Count; i++)
            {
                sufferingIndex += (numberOfEnslavedPeople[i] * materialImportSources.ImportRatios[i]);
            } 

            return new Output<double, List<string>, List<string>>
            {
                Item1 = sufferingIndex,
                Item2 = culledCountries,
                Item3 = invalidCountries
            };
        }
        /***************************************************/

    }
}