const disc = require("discord.js");

module.exports.error = (reason, origin, line) => {
    var e = new disc.RichEmbed();
    e.setTitle(":x: Uh oh! I've experienced an error. Sorry bout' this!");
    e.setDescription("It appears that I've experienced an error! \n\n\nPlease contact my developers if this becomes an issue.\n\n\n Thanks for your understanding.");
    e.setColor("RED");

    //Error name parser.
    var reasons = { "syntaxCMD": "Alpha", "syntaxEVENT": "Bravo", "syntaxMAIN": "Charlie", "discordjsAPI":"Delta" };
    var origins = { "unexpected": "Virgina", "expected": "Oregon", "missing": "Country", "unhandled": "Roads", "discord": "Whumpus" };

    e.setFooter("ERROR CODE: " + reasons[reason] + "-" + origins[origin] + "-" + new Date().getDay());
    return e;
}

module.exports.permissions = () => {
    var e = new disc.RichEmbed();
    e.setTitle(":thinking: Hmm... It look's like you cannot run this command.");
    e.setDescription("You do not have the permissions to run this command!\n\nIf you feel that this is an error, please contact my developers!");
    e.setColor("RED");
    return e;
}

module.exports.senttodms = (type) => {
    if (type == "help") {
        var e = new disc.RichEmbed();
        e.setTitle(":white_check_mark: Sent my commands to your dm's!");
        e.setColor("GREEN");
        return e;
    }
}

module.exports.success = (type, chan) => {
    if (type == "setlogs") {
        var e = new disc.RichEmbed();
        e.setTitle(":white_check_mark: Logs channel has been set.");
        if (chan)
            e.setDescription("The logs channel has been set to " + chan);

        e.setColor("GREEN");
        return e;
    }
}