using MTCG.Models.ResponseObject;
using MTCG.Utilities.Exceptions.CustomExceptions;
using System;
using System.Text.Json;

public static class ExceptionHandler
{
    public static ResponseObject HandleException(Exception ex)
    {
        Console.WriteLine($"[Exception] {ex.Message}");

        return ex switch
        {
            InvalidPackageException => new ResponseObject(400, ex.Message),
            JsonException => new ResponseObject(400, "Invalid JSON structure."),
            InvalidTradeException => new ResponseObject(400, ex.Message),
            UnauthorizedException => new ResponseObject(401, "Unauthorized."),
            NotAdminException => new ResponseObject(403, "Admin rights required."),
            InvalidDeckSizeException => new ResponseObject(403, "Invalid deck size."),
            NotEnoughCoinsException => new ResponseObject(403, "Not enough coins."),
            TradeNotFoundException => new ResponseObject(404, "Trading deal not found."),
            UserNotFoundException => new ResponseObject(404, "User not found."),
            DeckIsNullException => new ResponseObject(204, "Deck is null."),
            NoPackageAvailableException => new ResponseObject(404, "No package available."),
            TradeAlreadyExistsException => new ResponseObject(409, ex.Message),
            UserAlreadyExistsException => new ResponseObject(409, "User already exists."),
            PackageConflictException => new ResponseObject(409, "Package conflict."),
            _ => new ResponseObject(500, "An unexpected error occurred.")
        };
    }
}
