import { Events, Guild, EmbedBuilder } from 'discord.js';
import { logger } from '../utils/logger.js';

export const guildCreateEvent = {
  name: Events.GuildCreate,
  async execute(guild: Guild) {
    logger.info(`Joined new guild: ${guild.name} (${guild.id})`);

    try {
      const owner = await guild.fetchOwner();
      
      const embed = new EmbedBuilder()
        .setColor('Gold')
        .setTitle('Welcome to Thien Khu Cultivation System')
        .setDescription(`Thank you for inviting the bot to **${guild.name}**!`)
        .addFields(
          { name: 'Onboarding', value: 'Users can begin their journey using `/cultivate`. By default, chat messages in any channel will reward them with Qi.' },
          { name: 'Configuration', value: 'The server configuration is automatically initialized. In the future, you will be able to customize specific "Chat-to-Earn" channels via a web dashboard or commands.' },
          { name: 'Support', value: 'If you encounter any issues, refer to the documentation or contact support.' }
        );

      await owner.send({ embeds: [embed] });
      logger.info(`Sent DM onboarding guide to owner of guild ${guild.id}`);
    } catch (error) {
      logger.error(`Failed to send onboarding DM to owner of guild ${guild.id}`, { error });
    }
  },
};
