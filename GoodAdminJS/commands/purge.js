const disc = require("discord.js");
const db = require("quick.db");
const casing = require("../data/core/api/logs.js");
const embeds = require("../data/core/api/embeds.js");

module.exports.details = { name: "Purge", usage: "!purge [Ammount] [Optional | Reason]", catagory: 4, sdesc: "Purge a certain amount of messages." }
module.exports.execute = (client, msg, args) => {
    if (!msg.guild)
        return msg.channel.send({ embed: embeds.onlyguild() });

    if (!msg.member.hasPermission("MANAGE_MESSAGES", true, true, true) || !db.get(msg.author.id + "." + msg.guild.id + ".permlvl") > 0)
        return msg.channel.send({ embed: embeds.permissions() });

    if (!args[0])
        return msg.channel.send({ embed: embeds.badArgs(msg, 'Ammount of messages', '!purge [Ammount of messages] [Optional Reason]') });

    if (!typeof args[0] == 'number')
        return msg.channel.send({ embed: embeds.badArgs(msg, 'Ammount of messages', '!purge [Ammount of messages] [Optional Reason]') });

    if (!msg.guild.me.hasPermission("MANAGE_MESSAGES", false, true, true))
        return msg.channel.send({ embed: embeds.noperms('Manage Messages') });

    if (args[0] > 100) {
        var em = new disc.RichEmbed();
        em.setTitle(":x: I can only delete 100 messages at a time!");
        em.setColor("RED");
        return msg.channel.send(em);
    }

    msg.channel.bulkDelete(args[0]).then(messages => {
        var e = embeds.success('purge', msg.channel, messages.size);
        msg.channel.send({ embed: e });
        casing.addCase('purge', msg.guild, msg.author, null, msg.channel);
    })
}