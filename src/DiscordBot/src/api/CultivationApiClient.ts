// Wrapper for .NET Core REST API
export class CultivationApiClient {
    private readonly baseUrl: string;

    constructor() {
        // Fallback to localhost if not set in .env
        this.baseUrl = process.env.API_BASE_URL || 'http://localhost:5000/api';
    }

    // Example method to be implemented later
    // public async getCharacter(discordId: string) { ... }
}
