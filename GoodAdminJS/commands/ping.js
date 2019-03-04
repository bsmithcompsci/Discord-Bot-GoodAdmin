const disc = require("discord.js");
//Command details
module.exports.details = { name: "Ping", usage: "!ping", sdesc: "Makes the bot send you his ping.", ldesc: "Makes the bot send a detailed summary on the connection status of the bot's host and it's connection to Discord's API" }
module.exports.execute = (client, msg) => {
    var e = new disc.RichEmbed().setTitle(":ping_pong: Pong!").addField(":satellite: API Ping", client.ping + "ms").addField(":hourglass: Response Time", new Date() - msg.createdAt).setFooter("This was requested ").setTimestamp().setColor("BLUE");
    msg.channel.send(e).then(mess => { if (msg.deleteable) { msg.delete() } setTimeout(function () { mess.delete() }, 10000) });
}