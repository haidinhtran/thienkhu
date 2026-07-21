import { Client, Events } from 'discord.js';
import { logger } from '../utils/logger.js';

export const readyEvent = {
  name: Events.ClientReady,
  execute(client: Client) {
    logger.info(`Ready! Logged in as ${client.user?.tag}`);
  }
};
