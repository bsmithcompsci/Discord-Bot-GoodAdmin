const embeds = require("../data/core/api/embeds.js");
const db = require("quick.db") //Finally fuck yeah we got quick.db in the houseeeeee

module.exports.details = { name: "Setlogs", usage: "!setlogs", sdesc: "Sets the general logs channel to the channel that the command is executed in.", catagory: 4 };
module.exports.execute = (client, msg) => {
    if (!msg.guild)
        return;

    if (msg.member.hasPermission("MANAGE_CHANNELS", false, true, true) || db.get(msg.author.id + msg.guild.id) && db.get(msg.author.id + msg.guild.id).sadmin == true) {
        db.set(msg.guild.id + ".logs", msg.channel.id);
        msg.channel.send({ embed: embeds.success("setlogs", msg.channel) }).then(mess => { setTimeout(function () { mess.delete() }, 10000) });
    } else {
        return msg.channel.send({ embed: embeds.permissions() }).then(mess => { setTimeout(function () { mess.delete() }, 10000) });
    }
}