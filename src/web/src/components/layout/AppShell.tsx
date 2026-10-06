import { NavLink, Outlet } from 'react-router-dom';

export function AppShell() {
  return (
    <>
      <a className="skip-link" href="#main">
        Skip to main content
      </a>
      <header className="app-header">
        <span className="app-title">TempTrack</span>
        <nav aria-label="Primary">
          <ul>
            <li>
              <NavLink to="/">My requests</NavLink>
            </li>
          </ul>
        </nav>
      </header>
      <main id="main" tabIndex={-1}>
        <Outlet />
      </main>
    </>
  );
}
