///*
// * This file is part of the Buildings and Habitats object Model (BHoM)
// * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
// *
// * Each contributor holds copyright over their respective contributions.
// * The project versioning (Git) records all such contribution source information.
// *                                           
// *                                                                              
// * The BHoM is free software: you can redistribute it and/or modify         
// * it under the terms of the GNU Lesser General Public License as published by  
// * the Free Software Foundation, either version 3.0 of the License, or          
// * (at your option) any later version.                                          
// *                                                                              
// * The BHoM is distributed in the hope that it will be useful,              
// * but WITHOUT ANY WARRANTY; without even the implied warranty of               
// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
// * GNU Lesser General Public License for more details.                          
// *                                                                            
// * You should have received a copy of the GNU Lesser General Public License     
// * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
// */

//using BH.oM.Base.Attributes;
//using BH.oM.EmbodiedSuffering.Elements;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;

//namespace BH.Engine.EmbodiedSuffering
//{
//    public static partial class Compute
//    {
//        /***************************************************/
//        /****   Public Methods                          ****/
//        /***************************************************/

//        [Description("Calculate the FreedomOfAssociation for a specified country.")]
//        [Input("mass", "The total mass of the object or assembly.")]
//        [Input("labourExploitationRisk", "Labour Exploitation Risk data for a specific country.")]
//        [Input("materials", "Material Import Sources objects. Please provide a ratio summary for each material specified.")]
//        [Input("ratios", "Ratios of material that make up the assembly. This list length must account for each material you provide.")]
//        [Output("quantity", "This needs a description of how to interpret the output.")]
//        public static List<double> FreedomOfAssociation(double mass, LabourExploitationRisk labourRisk, List<MaterialImportSources>materials, List<double>ratios)
//        {
//            if (materials.Count() == ratios.Count)
//            {
//                List<double> massesPerRatio = new List<double>();
//                List<double> importRatios = new List<double>();
//                List<double> values = new List<double>();
//                List<double> valuesMass = new List<double>();

//                // FreedomOfAssociation from LabourExploitationRisk 
//                int freedomVar = labourRisk.FreedomOfAssociation;

//                // Get a list of import ratios from the materials
//                importRatios = materials.Select(x => x.ImportRatios).ToList();

//                // Return a list of Masses per ratio of material
//                for (int i = 0; i < materials.Count(); i++)
//                {
//                    // Create a list of masses per ratio
//                    massesPerRatio.Add(mass * ratios[i]);

//                    // Put all of the values into a list that can be added together
//                    values.Add(freedomVar * ratios[i] * importRatios[i]);

//                    valuesMass.Add(values[i] * massesPerRatio[i]);
//                }

//                // Return the list of Values modified by the Mass data found within the assembly
//                return valuesMass;
//            }
//            else
//            {
//                Base.Compute.RecordError("You must provide the same number of ratios for the number of materials. Returning NaN.");
//                return new List<double>();
//            }
//        }
//        /***************************************************/
//    }
//}



