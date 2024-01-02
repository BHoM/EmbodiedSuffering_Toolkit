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
using BH.Engine.Library;

namespace BH.Engine.EmbodiedSuffering
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the material import source for a material to a country as the avarage for the provided country from a dataset.")]
        [Input("material", "The material type to get the default import sources for.")]
        [Input("importCountry", "The country to which the material is being imported.")]
        [Input("importSourceType", "Control for if the import source should be by cost, mass or any.")]
        [Output("importSource", "The default MaterialImputSource record for the material and country specified.")]
        public static MaterialImportSources MaterialImportSource(this Material material, Country importCountry = Country.UnitedStatesOfAmerica, ImportSourceType importSourceType = ImportSourceType.ByMass)
        {
            string mainPath = "EmbodiedSuffering\\Material Imports";
            List<string> libraryNames = Library.Query.LibraryNames().Where(x => x.StartsWith(mainPath)).ToList();   //Get all available libraries

            List<string> librariesToBeUsed;

            //Filter by import source type
            switch (importSourceType)
            {
                case ImportSourceType.ByCost:
                    librariesToBeUsed = libraryNames.Where(x => x.ToUpper().Contains("BYCOST")).ToList();
                    break;
                case ImportSourceType.ByMass:
                    librariesToBeUsed = libraryNames.Where(x => x.ToUpper().Contains("BYMASS")).ToList();
                    break;
                case ImportSourceType.Undefined:
                default:
                    librariesToBeUsed = libraryNames;
                    break;
            }
            //Get all data
            List<MaterialImportSources> importSources = librariesToBeUsed.SelectMany(x => Library.Query.Library(x)).OfType<MaterialImportSources>().ToList();

            if (importSources.Count == 0)
            {
                Engine.Base.Compute.RecordError("No import source datasets available.");
                return null;
            }

            //Filter by type
            importSources = importSources.Where(x => x.Material == material).ToList();

            if (importSources.Count == 0)
            {
                Engine.Base.Compute.RecordError($"No import source datasets available for the material type {material}.");
                return null;
            }

            //Filter by import country
            importSources = importSources.Where(x => x.ImportCountry == importCountry).ToList();

            if (importSources.Count == 1)
                return importSources[0];
            else if (importSources.Count == 0)
            {
                Engine.Base.Compute.RecordError($"No import source datasets available for the import country {importCountry} for the material type {material}.");
                return null;
            }
            else
            {
                Engine.Base.Compute.RecordWarning($"More than one record found for import of material {material} to the country {importCountry}. Average of all available records returned.");

                Dictionary<Country, double> importRatios = new Dictionary<Country, double>();
                foreach (MaterialImportSources importSource in importSources)
                {
                    for (int i = 0; i < importSource.ExportCountries.Count; i++)
                    {
                        Country country = importSource.ExportCountries[i];
                        double ratio = importSource.ImportRatios[i];
                        if (importRatios.ContainsKey(country))
                            importRatios[country] += ratio;
                        else
                            importRatios[country] = ratio;
                    }
                }
                double count = importRatios.Count;
                return new MaterialImportSources
                {
                    ImportCountry = importCountry,
                    Material = material,
                    ExportCountries = importRatios.Keys.ToList(),
                    ImportRatios = importRatios.Values.Select(x => x / count).ToList()
                };
            }
        }

        /***************************************************/
    }
}


