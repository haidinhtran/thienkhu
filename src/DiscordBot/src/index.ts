import { Client, GatewayIntentBits } from 'discord.js';
import * as dotenv from 'dotenv';

// Load environment variables
dotenv.config();

const client = new Client({
    intents: [
        GatewayIntentBits.Guilds,
        GatewayIntentBits.GuildMessages,
        GatewayIntentBits.MessageContent
    ]
});

client.once('ready', () => {
    console.log(`Bot is ready! Logged in as ${client.user?.tag}`);
});

// Top-level error handling to prevent process crash
process.on('unhandledRejection', (error) => {
    console.error('Unhandled promise rejection:', error);
});

client.on('interactionCreate', async (interaction) => {
    if (!interaction.isChatInputCommand() && !interaction.isButton()) return;
    
    try {
        // Route interactions to respective controllers here
        // Example: if (interaction.commandName === 'tutien') { ... }
    } catch (error) {
        console.error('Error handling interaction:', error);
        
        // Respond with ephemeral error to avoid information leakage
        if (interaction.isRepliable() && !interaction.replied && !interaction.deferred) {
            await interaction.reply({ 
                content: 'There was an internal error executing this command. Please try again later.', 
                ephemeral: true 
            });
        }
    }
});

// Login
const token = process.env.DISCORD_TOKEN;
if (!token) {
    console.error('DISCORD_TOKEN is missing in environment variables.');
    process.exit(1);
}

client.login(token);
