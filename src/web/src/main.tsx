import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { App } from './App';
import { loadConfig } from './config';
import './styles.css';

const root = createRoot(document.getElementById('root')!);

// The API client is built from config in a later task once features consume it.
loadConfig()
  .then(() => {
    root.render(
      <StrictMode>
        <App />
      </StrictMode>,
    );
  })
  .catch((err: unknown) => {
    root.render(<p role="alert">TempTrack could not start: {String(err)}</p>);
  });
