import dotenv from 'dotenv';

dotenv.config();

export const env = {
  DISCORD_TOKEN: process.env.DISCORD_TOKEN || '',
  DISCORD_CLIENT_ID: process.env.DISCORD_CLIENT_ID || '',
  DISCORD_GUILD_ID: process.env.DISCORD_GUILD_ID || '',
  API_BASE_URL: process.env.API_BASE_URL || 'http://localhost:5000/api',
  LOG_LEVEL: process.env.LOG_LEVEL || 'info',
};
