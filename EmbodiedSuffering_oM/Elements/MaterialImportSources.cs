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

using BH.oM.Base;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.oM.EmbodiedSuffering.Elements
{
    [Description("An Embodied Suffering object used for defining the import ratios per country for a particular material, utilised in a specific country (e.g. the United States gets all of its timber from Brazil, or that it is 50% from Brazil and 50% from Vietnam.)")]
    public class MaterialImportSources : BHoMObject
    {
        [Description("The name of the material that is imported to a particular country from other countries.")]
        public virtual Material Material { get; set; } = Material.Undefined;

        [Description("List of countries from which the material was imported.")]
        public virtual List<Country> ExportCountries { get; set; } = new List<Country>();

        [Description("List of material import ratios from each country. For example, if Brazil is the sole country of import for timber the value would be 1.0, if the United Kingdom is responsible for 50% of the imports of steel to a particular country that value would be 0.5. The values do not necessarily need to add to 1.0.")]
        public virtual double ImportRatios { get; set; } = double.NaN;

        [Description("The name of the country in which the imported materials are utilised.")]
        public virtual Country ImportCountry { get; set; } = Country.Undefined; // Wouldn't this need to be a list to match the possibility of multiple import ratios? 
    }
}
