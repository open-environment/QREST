using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;

namespace QRESTModel.Units
{
    public class UnitConvert
    {
        public static double? ConvertUnit(double inputVal, string inputUnit, string outputUnit) {

            //no unit conversion needed
            if (inputUnit == outputUnit)
                return inputVal;

            //************* VOLUME CONCENTRATION *******************************
            if (inputUnit == "007")  //ppm
            {
                VolumeConcentration x = VolumeConcentration.FromPartsPerMillion(inputVal);
                if (outputUnit == "008")
                    return x.PartsPerBillion;
                if (outputUnit == "121")
                    return x.PartsPerTrillion;
            }
            else if (inputUnit == "008")  //ppb
            {
                VolumeConcentration x = VolumeConcentration.FromPartsPerBillion(inputVal);
                if (outputUnit == "007")
                    return x.PartsPerMillion;
                if (outputUnit == "121")
                    return x.PartsPerTrillion;
            }



            //************* MASS CONCENTRATION *******************************
            else if (inputUnit == "001")   // ug/m3
            {
                MassConcentration x = MassConcentration.FromMicrogramsPerCubicMeter(inputVal);
                if (outputUnit == "005")
                    return x.MilligramsPerCubicMeter;
            }
            else if (inputUnit == "105")  // ug/m3
            {
                MassConcentration x = MassConcentration.FromMicrogramsPerCubicMeter(inputVal);
                if (outputUnit == "109")
                    return x.MilligramsPerCubicMeter;
            }
            else if (inputUnit == "005")  // mg/m3
            {
                MassConcentration x = MassConcentration.FromMilligramsPerCubicMeter(inputVal);
                if (outputUnit == "001")
                    return x.MilligramsPerCubicMeter;
            }
            else if (inputUnit == "109")  // mg/m3
            {
                MassConcentration x = MassConcentration.FromMilligramsPerCubicMeter(inputVal);
                if (outputUnit == "105")
                    return x.MilligramsPerCubicMeter;
            }



            //************* SPEED*******************************
            else if (inputUnit == "011")  // m/s
            {
                Speed x = Speed.FromMetersPerSecond(inputVal);
                if (outputUnit == "012")
                    return x.MilesPerHour;
                if (outputUnit == "013")
                    return x.Knots;
                if (outputUnit == "060")
                    return x.KilometersPerHour;
            }
            else if (inputUnit == "012")  // mph
            {
                Speed x = Speed.FromMilesPerHour(inputVal);
                if (outputUnit == "011")
                    return x.MetersPerSecond;
                if (outputUnit == "013")
                    return x.Knots;
                if (outputUnit == "060")
                    return x.KilometersPerHour;
            }
            else if (inputUnit == "013")  // knots
            {
                Speed x = Speed.FromKnots(inputVal);
                if (outputUnit == "011")
                    return x.MetersPerSecond;
                if (outputUnit == "012")
                    return x.MilesPerHour;
                if (outputUnit == "060")
                    return x.KilometersPerHour;
            }
            else if (inputUnit == "013")  // kph
            {
                Speed x = Speed.FromKilometersPerHour(inputVal);
                if (outputUnit == "011")
                    return x.MetersPerSecond;
                if (outputUnit == "012")
                    return x.MilesPerHour;
                if (outputUnit == "013")
                    return x.Knots;
            }



            //************* TEMPERATURE*******************************
            else if (inputUnit == "015")  // degF
            {
                Temperature x = Temperature.FromDegreesFahrenheit(inputVal);
                if (outputUnit == "017")
                    return x.DegreesCelsius;
                if (outputUnit == "037")
                    return x.Kelvins;
            }
            else if (inputUnit == "017")  // degC
            {
                Temperature x = Temperature.FromDegreesCelsius(inputVal);
                if (outputUnit == "015")
                    return x.DegreesFahrenheit;
                if (outputUnit == "037")
                    return x.Kelvins;
            }
            else if (inputUnit == "037")  // K
            {
                Temperature x = Temperature.FromKelvins(inputVal);
                if (outputUnit == "015")
                    return x.DegreesFahrenheit;
                if (outputUnit == "017")
                    return x.DegreesCelsius;
            }



            //************* PRESSURE*******************************
            else if (inputUnit == "016")  // millibars
            {
                Pressure x = Pressure.FromMillibars(inputVal);
                if (outputUnit == "022")
                    return x.InchesOfMercury;
                if (outputUnit == "059")
                    return x.MillimetersOfMercury;
            }
            else if (inputUnit == "022")  // inHg
            {
                Pressure x = Pressure.FromInchesOfMercury(inputVal);
                if (outputUnit == "016")
                    return x.Millibars;
                if (outputUnit == "059")
                    return x.MillimetersOfMercury;
            }
            else if (inputUnit == "059")  // mmHg
            {
                Pressure x = Pressure.FromMillimetersOfMercury(inputVal);
                if (outputUnit == "016")
                    return x.Millibars;
                if (outputUnit == "022")
                    return x.InchesOfMercury;
            }



            //************* LENGTH*******************************
            else if (inputUnit == "021")  // in
            {
                Length x = Length.FromInches(inputVal);
                if (outputUnit == "029")
                    return x.Millimeters;
            }
            else if (inputUnit == "029")  // mm
            {
                Length x = Length.FromMillimeters(inputVal);
                if (outputUnit == "021")
                    return x.Inches;
            }



            //************* SOLAR RADIATION*******************************
            else if (inputUnit == "025")  // langley/min
            {
                if (outputUnit == "079")
                    return inputVal * (double)697.3;
            }
            else if (inputUnit == "079")  // W/m2
            {
                if (outputUnit == "025")
                    return inputVal / (double)697.3;
            }



            //if got this far, failed or cannot convert
            return null;
        }

    }
}
