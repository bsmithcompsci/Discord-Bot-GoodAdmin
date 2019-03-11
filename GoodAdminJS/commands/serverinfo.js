const db = require("quick.db");
const disc = require("discord.js");

module.exports.details = { name: "ServerInfo", usage: "!serverinfo", sdesc: "Get the servers information.", catagory: 4 };
module.exports.execute = (client, msg) => {
    if (!msg.guild)
        return;

    var guildE = db.get(msg.guild.id);
    var guild = msg.guild;

    var humans;
    var bots;
    var total;

    for (i in guild.members) {
        var mem = guild.members[i];
        if (mem.bot) {
            bots++;
        } else {
            humans++;
        }
    }

    total = humans + bots;

    var e = new disc.RichEmbed();
    e.setTitle(":pencil: Server information");
    if (guildE.logs) {
        e.addField()
    }
}