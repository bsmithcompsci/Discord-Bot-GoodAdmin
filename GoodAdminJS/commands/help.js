const disc = require("discord.js");
const fs = require("fs");
const api = require("../data/core/api/embeds.js");

module.exports.details = { name: "Help", usage: "!help", catagory: 1, sdesc: "Shows all commands currently registered." };
module.exports.execute = (client, msg, args, odata) => {
    var e = new disc.RichEmbed()
    e.setTitle(":pencil: Commands");
    var general = "";
    var moderation = "";
    var guild = "";
    var misc = "";
    console.log(odata)
    for (i in odata.cmds) {
        console.log(odata.cmds[i]);
        var command = odata.cmds[i].data.details;

        if (command.catagory == 1 && command.hidden != true) {
            general += command.name + " | " + command.usage + " | " + command.sdesc + "\n";
        } else if (command.catagory == 2 && command.hidden != true) {
            moderation += command.name + " | " + command.usage + " | " + command.sdesc + "\n";
        } else if (command.catagory == 3 && command.hidden != true) {
            misc += command.name + " | " + command.usage + " | " + command.sdesc + "\n";
        } else if (command.catagory == 4 && command.hidden != true) {
            guild += command.name + " | " + command.usage + " | " + command.sdesc + "\n";
        }

        if (i > odata.cmds.length || i == odata.cmds.length || i == odata.cmds.length - 1) {
            console.log("Finished.")
            if (general == "") {
                general = "None registered.";
            }
            if (moderation == "") {
                moderation = "None registered.";
            }
            if (misc == "") {
                misc = "None registered.";
            }
            if (guild == "") {
                guild = "None registered";
            }

            e.addField(":bookmark: General Commands:", general);
            e.addField(":shield: Moderation Commands:", moderation);
            e.addField(":gear: Server Commands:", guild);
            e.addField(":stuck_out_tongue_winking_eye: Misc Commands:", misc);
            e.setColor("BLUE");
            msg.author.send({ embed: e }).then(mess => { msg.channel.send({ embed: api.senttodms("help") }); if (msg.deleteable) { msg.delete() }; setTimeout(function () { mess.delete() }, 120000); }).catch(er => {
                e.setFooter("Your DM settings prevented me from sending it to your dm's, so the help was sent in the channel.");
                msg.channel.send({ embed: e }).then(mess => {
                    setTimeout(function () {
                        mess.delete()
                    }, 60000)
                });
            });
        }
    }
}