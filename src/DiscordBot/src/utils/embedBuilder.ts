import { EmbedBuilder } from 'discord.js';

export const embedBuilder = {
  buildErrorEmbed: (message: string, traceId: string) => {
    return new EmbedBuilder()
      .setColor('Red')
      .setTitle('Error Occurred')
      .setDescription(message)
      .setFooter({ text: `Trace ID: ${traceId}` });
  },

  buildMainMenuEmbed: (username: string) => {
    return new EmbedBuilder()
      .setColor('Blue')
      .setTitle('Cultivation Main Menu')
      .setDescription(`Welcome back, ${username}. What would you like to do?`);
  },

  buildProfileEmbed: (profile: any) => {
    return new EmbedBuilder()
      .setColor('Gold')
      .setTitle(`${profile.username}'s Profile`)
      .addFields(
        { name: 'Realm', value: `${profile.realmName} (Lv. ${profile.level})`, inline: true },
        { name: 'Qi', value: `${profile.currentQi} / ${profile.dailyQiLimit}`, inline: true },
        { name: 'Spirit Stones', value: `${profile.spiritStones}`, inline: true },
        { name: 'Status', value: `${profile.currentState}`, inline: false }
      );
  },

  buildLocationSelectEmbed: () => {
    return new EmbedBuilder()
      .setColor('Green')
      .setTitle('Exploration')
      .setDescription('Select a location to explore. Higher level areas offer greater rewards but higher risks.');
  },

  buildExplorationEventEmbed: (eventData: any) => {
    const embed = new EmbedBuilder()
      .setColor(eventData.eventType === 'COMBAT' ? 'Red' : 'Purple')
      .setTitle(eventData.title)
      .setDescription(eventData.description);
    if (eventData.imageUrl) {
      embed.setImage(eventData.imageUrl);
    }
    return embed;
  },

  buildExplorationResultEmbed: (resultData: any) => {
    const embed = new EmbedBuilder()
      .setColor(resultData.success ? 'Green' : 'DarkRed')
      .setTitle(resultData.title)
      .setDescription(resultData.narrative);

    if (resultData.qiReward > 0 || resultData.spiritStonesReward > 0) {
      embed.addFields({ 
        name: 'Rewards', 
        value: `Qi: +${resultData.qiReward}\nSpirit Stones: +${resultData.spiritStonesReward}` 
      });
    }

    return embed;
  },

  buildAscendResultEmbed: (resultData: any) => {
    const embed = new EmbedBuilder()
      .setColor(resultData.success ? 'Gold' : 'Red')
      .setTitle(resultData.success ? 'Ascension Successful!' : 'Ascension Failed')
      .setDescription(resultData.message);

    if (resultData.success && resultData.newBaseStats) {
      embed.addFields(
        { name: 'Level Up', value: `Lv. ${resultData.oldLevel} -> Lv. ${resultData.newLevel}`, inline: false },
        { name: 'Strength', value: `${resultData.newBaseStats.strength}`, inline: true },
        { name: 'Agility', value: `${resultData.newBaseStats.agility}`, inline: true },
        { name: 'Health', value: `${resultData.newBaseStats.health}`, inline: true }
      );
    }
    return embed;
  },

  buildSecretDomainResultEmbed: (resultData: any) => {
    const embed = new EmbedBuilder()
      .setColor(resultData.isVictory ? 'Orange' : 'DarkButNotBlack')
      .setTitle(resultData.isVictory ? 'Secret Domain Cleared' : 'Defeated in Secret Domain')
      .setDescription(resultData.battleLog.join('\n'));

    if (resultData.isVictory) {
      let rewardText = `Spirit Stones: +${resultData.rewardSpiritStones}\n`;
      if (resultData.rewardItems && resultData.rewardItems.length > 0) {
        resultData.rewardItems.forEach((i: any) => {
          rewardText += `${i.itemId} x${i.quantity}\n`;
        });
      } else {
        rewardText += "No items dropped.";
      }
      embed.addFields({ name: 'Rewards', value: rewardText });
    }
    return embed;
  }
};
