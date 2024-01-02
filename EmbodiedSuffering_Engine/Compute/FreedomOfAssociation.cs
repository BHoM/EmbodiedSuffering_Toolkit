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

using BH.oM.Base.Attributes;
using BH.oM.Base;
using BH.oM.EmbodiedSuffering.Elements;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System;

namespace BH.Engine.EmbodiedSuffering
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Calculate the FreedomOfAssociation based on the average import ratios to the specified country for a set of provided Materials and Ratios..")]
        [Input("materials", "Material to be used. Average values for import ratios of the material to the selected import country will be queried from datasets. Please provide a ratio summary for each material specified.")]
        [Input("ratios", "Ratios of material that make up the assembly. This list length must account for each Material you provide.")]
        [Input("importCountry", "The country to which the material is being imported.")]
        [Input("importSourceType", "Control for if the import source should be by cost, mass or the average value of cost and mass.")]
        [MultiOutput(0,"freedom", "")]
        [MultiOutput(1, "missingCountries", "Countries from which no data is avialable.")]
        [MultiOutput(2, "missingRatio", "The total ratio not included due to missing FreedomOfAssociation values for the particular country.")]
        public static Output<double, List<Country>, double> FreedomOfAssociation(List<Material> materials, List<double> ratios, Country importCountry = Country.UnitedStatesOfAmerica, ImportSourceType importSourceType = ImportSourceType.ByMass)
        {
            if (materials.Count != ratios.Count)
            {
                Base.Compute.RecordError("You must provide the same number of ratios for the number of Material. Returning NaN.");
                return new Output<double, List<Country>, double> { Item1 = double.NaN, Item2 = new List<Country>(), Item3 = 0 };
            }

            if (materials.Count == 0)
            {
                Base.Compute.RecordError("No Material provided. NaN value returned.");
                return new Output<double, List<Country>, double> { Item1 = double.NaN, Item2 = new List<Country>(), Item3 = 0 };
            }

            List<MaterialImportSources> importSources = materials.Select(x => x.MaterialImportSource(importCountry, importSourceType)).ToList();

            if (importSources.Any(x => x == null))
                return new Output<double, List<Country>, double> { Item1 = double.NaN, Item2 = new List<Country>(), Item3 = 0 };

            return FreedomOfAssociation(importSources, ratios);
        }

        /***************************************************/

        [Description("Calculate the FreedomOfAssociation for a set of provided MaterialImportSources.")]
        [Input("materialImportSources", "Material Import Sources objects. Please provide a ratio summary for each material specified.")]
        [Input("ratios", "Ratios of material that make up the assembly. This list length must account for each MaterialImportSources you provide.")]
        [MultiOutput(0, "freedom", "")]
        [MultiOutput(1, "missingCountries", "Countries from which no data is available.")]
        [MultiOutput(2, "missingRatio", "The total ratio not included due to missing FreedomOfAssociation values for the particular country.")]
        public static Output<double, List<Country>, double> FreedomOfAssociation(List<MaterialImportSources> materialImportSources, List<double> ratios)
        {
            if (materialImportSources.Count != ratios.Count)
            {
                Base.Compute.RecordError("You must provide the same number of ratios for the number of MaterialImportSources. Returning NaN.");
                return new Output<double, List<Country>, double> { Item1 = double.NaN, Item2 = new List<Country>(), Item3 = 0 };
            }

            if (materialImportSources.Count == 0)
            {
                Base.Compute.RecordError("No materialImportSources provided. NaN value returned.");
                return new Output<double, List<Country>, double> { Item1 = double.NaN, Item2 = new List<Country>(), Item3 = 0 };
            }

            double value = 0;
            double ratiosSum = ratios.Sum();

            if (ratiosSum == 0)
            {
                Engine.Base.Compute.RecordError("Total ratios are summing to 0. Unable to compute Freedom of Association value. Raturning NaN.");
                return new Output<double, List<Country>, double> { Item1 = double.NaN, Item2 = new List<Country>(), Item3 = 0 };
            }

            if (Math.Abs(1 - ratiosSum) > 0.01)    //Ratios provided is less that 99%, raise warning
                Engine.Base.Compute.RecordWarning($"Ratios provided make up {ratiosSum}, Value returned will be weighted as if the total ratios sum equals 1.");

            HashSet<Country> missingCountries = new HashSet<Country>();
            double missingRatio = 0;
            for (int i = 0; i < materialImportSources.Count; i++)
            {
                Output<double, List<Country>, double> singleItemFreedom = materialImportSources[i].FreedomOfAssociation();
                
                value += singleItemFreedom.Item1 * ratios[i];
                missingRatio += singleItemFreedom.Item3 * ratios[i];

                foreach (Country country in singleItemFreedom.Item2)
                {
                    missingCountries.Add(country);
                }
            }

            value = value / ratiosSum;
            missingRatio = missingRatio / ratiosSum;
            return new Output<double, List<Country>, double>
            {
                Item1 = value,
                Item2 = missingCountries.ToList(),
                Item3 = missingRatio
            };
        }

        /***************************************************/

        [Description("Gets the Freedom of Association value from a MaterialImportSources object. The value will be weighted based on the ratios of the importing countries.")]
        [Input("materialImportSource", "The material import source to extract the freedom of association value for.")]
        [MultiOutput(0, "freedom", "")]
        [MultiOutput(1, "missingCountries", "Countries from which no data is avialable.")]
        [MultiOutput(2, "missingRatio", "The total ratio not included due to missing FreedomOfAssociation values for the particular country.")]
        public static Output<double, List<Country>, double> FreedomOfAssociation(this MaterialImportSources materialImportSource)
        {
            if (materialImportSource == null)
            {
                Engine.Base.Compute.RecordError("Cannot get the freedom of association value from a null MaterialImportSources.");
                return new Output<double, List<Country>, double> { Item1 = double.NaN, Item2 = new List<Country>(), Item3 = 0 };
            }

            if (materialImportSource.ExportCountries.Count != materialImportSource.ImportRatios.Count)
            {
                Engine.Base.Compute.RecordError("Import ratio counts need to be the same as export country count.");
                return new Output<double, List<Country>, double> { Item1 = double.NaN, Item2 = new List<Country>(), Item3 = 0 };
            }

            Dictionary<Country, int> freedomOfAssociations = Query.ITUCFreedomOfAssociationDictionary();

            double totalRatioUsed = 0;
            double missingRatio = 0;
            List<Country> missingCountries = new List<Country>();

            double value = 0;

            for (int i = 0; i < materialImportSource.ExportCountries.Count; i++)
            {
                Country exportCountry = materialImportSource.ExportCountries[i];
                double ratio = materialImportSource.ImportRatios[i];

                int freedom;
                if (!freedomOfAssociations.TryGetValue(exportCountry, out freedom))
                {
                    missingRatio += ratio;
                    missingCountries.Add(exportCountry);
                }
                else
                {
                    value += ((double)freedom) * ratio;
                    totalRatioUsed += ratio;
                }
            }

            if (totalRatioUsed == 0)
            {
                Base.Compute.RecordError($"No freedom of association values found for any of the export countries in the provided {nameof(MaterialImportSources)} for the import country {materialImportSource.ImportCountry} for material of type {materialImportSource.Material}");
                return new Output<double, List<Country>, double> { Item1 = double.NaN, Item2 = missingCountries, Item3 = 1.0 };
            }

            if (missingCountries.Count != 0)
                Base.Compute.RecordWarning($"No freedom of association values available for the following countries {string.Join(", ", missingCountries)}, making up {missingRatio * 100}% of the {nameof(MaterialImportSources)} for the import country {materialImportSource.ImportCountry} for material of type {materialImportSource.Material}.");

            if (Math.Abs(1 - totalRatioUsed) > 0.01)   //Less than 99% used
                Base.Compute.RecordWarning($"The freedom of association value returned from the {nameof(MaterialImportSources)} for the import country {materialImportSource.ImportCountry} for material of type {materialImportSource.Material} is based on a total of {totalRatioUsed * 100}% of the import ratios.");

            value = value / totalRatioUsed;
            double ratioMissed = missingRatio / (missingRatio + totalRatioUsed);    //Normalise the missing ratio in case of all ratios not summing to 1.

            return new Output<double, List<Country>, double>
            {
                Item1 = value,
                Item2 = missingCountries,
                Item3 = ratioMissed
            };
        }

        /***************************************************/
    }
}





