const disc = require("discord.js");
const db = require("quick.db");
const embeds = require("../data/core/api/embeds.js");

module.exports.details = { name: "Addmod", usage: "!addmod [Mention User]", catagory: 4, sdesc: "Adds a server moderator." };
module.exports.execute = (client, msg, args) => {
    if (msg.guild) {
        if (msg.member.hasPermission("ADMINISTRATOR", false, true, true) || db.get(msg.author.id + "." + msg.guild.id + ".permlvl") == 3) {
            if (!msg.mentions.members.array()[0]) {
                return msg.channel.send({ embed: embeds.badArgs(msg, 'User Mention', '!addmod @User#2000') });
            }
            var user = msg.mentions.members.array()[0]
            db.set(user.user.id + "." + msg.guild.id + ".permlvl", 1);

            var e = new disc.RichEmbed();
            e.setTitle(":shield: Added a moderator!");
            e.setDescription(user + " was added as a moderator to this guild.");
            e.setColor("GREEN");

            msg.channel.send({ embed: e });
        } else {
            return msg.channel.send({ embed: embeds.permissions() })
        }
    } else {
        return msg.channel.send({ embed: embeds.onlyguild() })
    }
}