using DotNetTask_RokasG.Constants.DefaultLocations;
using DotNetTask_RokasG.UserInput.DTOs;

public static class UserInputHandler
{
    public static UserInputLocation GetUserLocationOrDefault()
    {
        Console.WriteLine("\nWould you like to choose a custom location? Press 'y' for yes, any other key to skip.");
        var addLocationInput = Console.ReadKey(intercept: true).KeyChar;

        if (addLocationInput == 'y')
        {
            return GetCustomLocation();
        }

        return new UserInputLocation { Latitude = Locations.VILNIUS.Latitude, Longitude = Locations.VILNIUS.Longitude };
    }

    private static UserInputLocation GetCustomLocation()
    {
        var latitude = GetLatitude();
        var longitude = GetLongitude();

        Console.WriteLine($"Custom location added: Latitude {latitude}, Longitude {longitude}");
        return new UserInputLocation { Latitude = latitude, Longitude = longitude };
    }

    private static float GetLatitude()
    {
        float latitude = 0;
        Console.WriteLine("\nEnter Latitude:");

        while (true)
        {
            if (float.TryParse(Console.ReadLine(), out latitude) && latitude >= -90 && latitude <= 90)
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid input for Latitude. Please enter a value between -90 and 90:");
            }
        }
        return latitude;
    }

    private static float GetLongitude()
    {
        float longitude = 0;
        Console.WriteLine("\nEnter Longitude:");

        while (true)
        {
            if (float.TryParse(Console.ReadLine(), out longitude) && longitude >= -180 && longitude <= 180)
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid input for Longitude. Please enter a value between -180 and 180:");
            }
        }
        return longitude;
    }
}
