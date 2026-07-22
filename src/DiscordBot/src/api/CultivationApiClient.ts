// Wrapper for .NET Core REST API
import { logger } from '../utils/logger.js';

export interface CharacterProfileDto {
  discordId: string;
  serverId: string;
  username: string;
  level: number;
  realmName: string;
  currentQi: number;
  dailyQiLimit: number;
  spiritStones: number;
  currentState: string;
}

export class CultivationApiClient {
  private readonly baseUrl: string;

  constructor() {
    // Fallback to localhost if not set in .env
    this.baseUrl = process.env.API_BASE_URL || 'http://localhost:5116/api/v1';
  }

  public async getCharacterProfile(discordId: string, serverId: string, username: string): Promise<CharacterProfileDto> {
    const url = new URL(`${this.baseUrl}/characters/profile`);
    url.searchParams.append('discordId', discordId);
    url.searchParams.append('serverId', serverId);
    url.searchParams.append('username', username);

    logger.info(`Fetching character profile for discordId: ${discordId}`, { url: url.toString() });

    try {
      const response = await fetch(url.toString(), {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json',
        },
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`API Error: ${response.status} ${response.statusText} - ${errorText}`);
      }

      const data = await response.json();
      return data as CharacterProfileDto;
    } catch (error) {
      logger.error('Failed to fetch character profile', { error });
      throw error;
    }
  }
}

export const apiClient = new CultivationApiClient();
