export interface AppConfig {
  apiUrl: string;
  authMode: 'mock' | 'entra';
}

// Runtime config lets one image serve every environment (architecture 7.3).
export async function loadConfig(): Promise<AppConfig> {
  const res = await fetch('/config.json', { cache: 'no-store' });
  if (!res.ok) throw new Error(`Failed to load /config.json (${res.status})`);
  return (await res.json()) as AppConfig;
}
