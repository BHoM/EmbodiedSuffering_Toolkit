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
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BH.oM.Base;


namespace BH.oM.EmbodiedSuffering.Elements
{
    [Description("An Embodied Suffering object used for defining the import ratios per country for a particular material, utilised in a specific country (e.g. the United States gets all of its timber from Brazil, or that it is 50% from Brazil and 50% from Vietnam.)")]
    public class MaterialImportSources : BHoMObject
    {
        [Description("List of countries from which the material was imported.")]
        public virtual Country ExportCountries { get; set; } = Country.Undefined;

        [Description("List of material import ratios from each country. For example, if Brazil is the sole country of import for timber the value would be 1.0, if the United Kingdom is repsponsible for 50% of the imports of steel to a particular country that value would be 0.5.")]
        public virtual List<double> ImportRatios { get; set; } = new List<double>();

        [Description("The name of the country in which the imported materials are utilised.")]
        public virtual Country ImportCountry { get; set; } = Country.Undefined;

    }
}
