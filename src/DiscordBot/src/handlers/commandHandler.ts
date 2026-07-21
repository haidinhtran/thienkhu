import { REST, Routes } from 'discord.js';
import { env } from '../config/env.js';
import { logger } from '../utils/logger.js';
import { cultivateCommand } from '../commands/cultivate.js';

export const registerCommands = async () => {
  if (!env.DISCORD_TOKEN || !env.DISCORD_CLIENT_ID || !env.DISCORD_GUILD_ID) {
    logger.warn('Missing DISCORD_TOKEN, DISCORD_CLIENT_ID, or DISCORD_GUILD_ID. Skipping command registration.');
    return;
  }

  const rest = new REST({ version: '10' }).setToken(env.DISCORD_TOKEN);
  const commands = [cultivateCommand.data.toJSON()];

  try {
    logger.info(`Started refreshing ${commands.length} application (/) commands for guild ${env.DISCORD_GUILD_ID}.`);

    await rest.put(
      Routes.applicationGuildCommands(env.DISCORD_CLIENT_ID, env.DISCORD_GUILD_ID),
      { body: commands }
    );

    logger.info('Successfully reloaded application (/) commands.');
  } catch (error) {
    logger.error('Error registering commands', { error });
  }
};
