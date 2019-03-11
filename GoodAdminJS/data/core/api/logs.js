const disc = require("discord.js");
const db = require("quick.db");
const ex = module.exports;

ex.addCase = (action, guild, executer, user, channel, reason) => {
    var num = db.get(guild.id + "case");
    num += 1;
    db.set(guild.id + "case", num);

    var Case = { number: num };

    if (reason) {
        Case.reason = reason;
    }

    if (user) {
        Case.user = user.id;
    }

    Case.action = action;
    Case.executer = executer.id;
    Case.guild = guild.id;

    var em = new disc.RichEmbed();
    em.setTitle("[:shield: | MODERATION] New action!");
    em.setDescription(executer + " has executed the " + action + " command.");
    if (user)
        em.addField("Victim:", user.tag, true);

    if (!reason) {
        em.setFooter("To add a reason to this case, do !reason " + num + " [Reason]");
    } else {
        em.addField("Reason:", reason, true);
        em.setFooter("Case Number: " + num);
    }

    if (channel) {
        em.addField("Channel:", channel, true);
        Case.channel = channel.id;
    }

    em.setColor("BLUE");

    if (db.get(guild.id + ".logs")) {
        guild.channels.find("id", db.get(guild.id + ".logs")).send({ embed: em }).then(mess => {
            Case.embedid = mess.id;
            db.set(guild.id + "." + num + "case", Case);
        })
    }
}

module.exports.addReason = (c, guild, caseid, reason) => {
    var Case = db.get(guild.id + "." + caseid + "case")
    if (!Case) {
        return "abort:nocase"
    }
    Case.reason = reason;
    var message = c.guilds.find('id', Case.guild).channels.find('id', Case.channel).fetchMessage('id', Case.embedid);
    var embed = new disc.RichEmbed(message.embeds.array()[0]);
    embed.setFooter("Reason: '" + reason + "' by " + Case.user);
    embed.setTimestamp();
    message.edit(embed);
}