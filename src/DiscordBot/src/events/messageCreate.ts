import { Events, Message } from 'discord.js';
import { logger } from '../utils/logger.js';
import { apiClient } from '../api/CultivationApiClient.js';

export const messageCreateEvent = {
  name: Events.MessageCreate,
  async execute(message: Message) {
    if (message.author.bot) return;
    if (!message.guild) return;

    const guildId = message.guild.id;
    const authorId = message.author.id;
    const authorUsername = message.author.username;

    try {
      // 1. Fetch server config
      const serverConfig = await apiClient.getServerConfig(guildId);
      
      if (!serverConfig.isActive) return;

      // 2. Channel Guard & Fallback Logic
      let isAllowed = false;
      const configuredChannels = serverConfig.chatToEarnChannels || [];

      if (configuredChannels.length > 0) {
        if (configuredChannels.includes(message.channelId)) {
          isAllowed = true;
        }
      } else {
        // Fallback to determine default text channel
        // e.g. systemChannel or first accessible GuildText channel
        let fallbackChannelId = message.guild.systemChannelId;
        
        if (!fallbackChannelId) {
          const channels = await message.guild.channels.fetch();
          const firstTextChannel = channels.find(c => c && c.isTextBased());
          if (firstTextChannel) {
            fallbackChannelId = firstTextChannel.id;
          }
        }

        if (fallbackChannelId) {
          if (message.channelId === fallbackChannelId) {
            isAllowed = true;
          }
        } else {
          // No explicit channels configured AND no fallback found -> Disable feature
          return;
        }
      }

      if (!isAllowed) return;

      // 3. Trigger backend asynchronously (fire and forget)
      apiClient.gainQi(authorId, guildId, authorUsername)
        .then(result => {
          if (result.success && result.gainedQi > 0) {
            // Optional: Reaction or silent logging. We don't want to spam chat.
            logger.info(`User ${authorUsername} gained ${result.gainedQi} Qi in server ${guildId}`);
          }
        })
        .catch(err => {
          logger.error('Error triggering gainQi asynchronously', err);
        });

    } catch (error) {
      logger.error('Error handling messageCreate event', error);
    }
  },
};
