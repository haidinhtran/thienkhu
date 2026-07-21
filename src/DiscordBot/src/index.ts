import { Client, GatewayIntentBits } from 'discord.js';
import { env } from './config/env.js';
import { logger } from './utils/logger.js';
import { registerEvents } from './handlers/eventHandler.js';
import { registerCommands } from './handlers/commandHandler.js';

const client = new Client({ intents: [GatewayIntentBits.Guilds] });

async function bootstrap() {
  if (!env.DISCORD_TOKEN) {
    logger.error('DISCORD_TOKEN is missing!');
    process.exit(1);
  }

  try {
    await registerEvents(client);
    await registerCommands();
    await client.login(env.DISCORD_TOKEN);
  } catch (error) {
    logger.error('Failed to start the bot:', error);
    process.exit(1);
  }
}

bootstrap();
