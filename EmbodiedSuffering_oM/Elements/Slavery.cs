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
    [Description("An Embodied Suffering object used for defining the prevalence of modern slavery in each country's population. Measured by the number of victims per 1000 people.")]
    public class Slavery : BHoMObject
    {
        [Description("The number of enslaved people per 1000 population.")]
        public virtual double NumberOfEnslavedPeople { get; set; } = 0;

        [Description("Name of the country with which the modern slavery metrics are associated.")]
        public virtual Country Country { get; set; } = Country.Undefined;

        [Description("The name of the material with which the modern slavery metrics are associated. For example, the timber industry in Brazil. If no country is defined, and the metrics are general, this field shall be marked as undefined.")]
        public virtual Material Material { get; set; } = Material.Undefined;

    }
}