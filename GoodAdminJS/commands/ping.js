const disc = require("discord.js");
//Command details
module.exports.details = { name: "Ping", usage: "!ping", catagory: 1, sdesc: "Makes the bot send you his ping.", ldesc: "Makes the bot send a detailed summary on the connection status of the bot's host and it's connection to Discord's API" }
module.exports.execute = (client, msg) => {
    //Log for debug :("Ping fired");
    var em = new disc.RichEmbed();
    em.setTitle(":ping_pong: Pong!");
    em.setDescription("I was requested to give out my connection status'")
    em.addField(":satellite: API Ping", Math.round(client.ping) + "ms", true);
    em.addField(":hourglass: Response Time", msg.createdAt - new Date()+"ms", true);
    em.setFooter("This was requested ");
    em.setTimestamp();
    em.setColor("BLUE");
    //Log for debug :(em);
    if (!em) {em="empty message error"}
    msg.channel.send({ embed: em }).then(mess => { if (msg.deleteable) { msg.delete() }; setTimeout(function () { mess.delete() }, 10000); });
}