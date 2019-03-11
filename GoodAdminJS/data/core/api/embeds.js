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

module.exports.success = (type, chan, ammount) => {
    if (type == "setlogs") {
        var e = new disc.RichEmbed();
        e.setTitle(":white_check_mark: Logs channel has been set.");
        if (chan)
            e.setDescription("The logs channel has been set to " + chan);

        e.setColor("GREEN");
        return e;
    } else if (type == "purge") {
        var e = new disc.RichEmbed();
        e.setDescription(":white_check_mark: Successfuly deleted " + ammount + " messages in " + chan);
        e.setColor("GREEN");
        return e;
    }
}

module.exports.badArgs = (message, missarg, usage) => {
    var e = new disc.RichEmbed();
    e.setTitle(":x: Bad arguments!");
    e.setDescription("You are missing the argument '" + missarg + "'.\nPlease use it like: " + usage);
    e.setColor("RED");
    return e;
}

module.exports.onlyguild = () => {
    var e = new disc.RichEmbed();
    e.setTitle(":x: This command can only be used in a guild.");
    e.setColor("RED");
    return e;
}

module.exports.noperms = (missingperm) => {
    var e = new disc.RichEmbed();
    e.setTitle(":x: Uh oh! I'm missing certain permissions!");
    e.setDescription("I seem to be missing the '" + missingperm + "' permission\n\nPlease contact the guild owner or administrator to fix this issue.");
    e.setColor("RED");
    return e;
}