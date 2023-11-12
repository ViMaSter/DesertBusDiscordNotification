using System.Diagnostics.CodeAnalysis;

namespace DesertBusDiscordNotification.Client.Models;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable NotAccessedPositionalProperty.Global
[ExcludeFromCodeCoverage(Justification = "POCO")]
public record GivewayResponse(
    Prize[] prizes,
    bool _user
);

[ExcludeFromCodeCoverage(Justification = "POCO")]
public record Prize(
    int id,
    string type,
    string state,
    string image,
    string title,
    decimal bid,
    string bidder,
    string description,
    string start,
    string end,
    Donor donor
);

[ExcludeFromCodeCoverage(Justification = "POCO")]
public record Donor(
    string name,
    string website,
    string tumblr,
    string twitter,
    string facebook,
    string instagram,
    string etsy,
    string mastodon
);
// ReSharper restore NotAccessedPositionalProperty.Global
// ReSharper restore ClassNeverInstantiated.Global
// ReSharper restore IdentifierTypo
// ReSharper restore InconsistentNaming