import { describe, it, expect, beforeEach, vi } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { AuthProvider, useAuth } from './AuthContext';
import { TOKEN_STORAGE_KEY } from '../services/api';
import { authService } from '../services/authService';

vi.mock('../services/authService', () => ({
  authService: { login: vi.fn() },
}));

function AuthProbe() {
  const { user, isAuthenticated, login, logout } = useAuth();
  return (
    <div>
      <span data-testid="is-authenticated">{String(isAuthenticated)}</span>
      <span data-testid="user-email">{user?.email ?? ''}</span>
      <button onClick={() => login({ email: 'admin@adedonha.com', password: 'senha123' }).catch(() => {})}>entrar</button>
      <button onClick={() => logout()}>sair</button>
    </div>
  );
}

function buildFakeToken(payload: Record<string, unknown>): string {
  const header = btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' }));
  const body = btoa(JSON.stringify(payload));
  return `${header}.${body}.assinatura-fake`;
}

describe('AuthContext', () => {
  beforeEach(() => {
    localStorage.clear();
    vi.mocked(authService.login).mockReset();
  });

  it('inicia sem usuário autenticado quando não há token salvo', async () => {
    render(<AuthProvider><AuthProbe /></AuthProvider>);
    await waitFor(() => expect(screen.getByTestId('is-authenticated').textContent).toBe('false'));
  });

  it('autentica e salva o token ao fazer login com sucesso', async () => {
    const token = buildFakeToken({ email: 'admin@adedonha.com', name: 'Admin', sub: '1', role: 'Admin', exp: 9999999999 });
    vi.mocked(authService.login).mockResolvedValue({ token, expiration: '2099-01-01T00:00:00Z' });

    render(<AuthProvider><AuthProbe /></AuthProvider>);
    await waitFor(() => expect(screen.getByTestId('is-authenticated').textContent).toBe('false'));

    fireEvent.click(screen.getByText('entrar'));

    await waitFor(() => expect(screen.getByTestId('is-authenticated').textContent).toBe('true'));
    expect(screen.getByTestId('user-email').textContent).toBe('admin@adedonha.com');
    expect(localStorage.getItem(TOKEN_STORAGE_KEY)).toBe(token);
  });

  it('não salva token quando o login falha', async () => {
    vi.mocked(authService.login).mockRejectedValue(new Error('Credenciais inválidas'));

    render(<AuthProvider><AuthProbe /></AuthProvider>);
    await waitFor(() => expect(screen.getByTestId('is-authenticated').textContent).toBe('false'));

    fireEvent.click(screen.getByText('entrar'));

    await waitFor(() => expect(localStorage.getItem(TOKEN_STORAGE_KEY)).toBeNull());
    expect(screen.getByTestId('is-authenticated').textContent).toBe('false');
  });

  it('limpa o usuário e o token ao fazer logout', async () => {
    const token = buildFakeToken({ email: 'admin@adedonha.com', name: 'Admin', sub: '1', role: 'Admin', exp: 9999999999 });
    vi.mocked(authService.login).mockResolvedValue({ token, expiration: '2099-01-01T00:00:00Z' });

    render(<AuthProvider><AuthProbe /></AuthProvider>);
    await waitFor(() => expect(screen.getByTestId('is-authenticated').textContent).toBe('false'));
    fireEvent.click(screen.getByText('entrar'));
    await waitFor(() => expect(screen.getByTestId('is-authenticated').textContent).toBe('true'));

    fireEvent.click(screen.getByText('sair'));

    expect(screen.getByTestId('is-authenticated').textContent).toBe('false');
    expect(localStorage.getItem(TOKEN_STORAGE_KEY)).toBeNull();
  });

  it('recupera o usuário a partir de um token já salvo no localStorage', async () => {
    const token = buildFakeToken({ email: 'admin@adedonha.com', name: 'Admin', sub: '1', role: 'Admin', exp: 9999999999 });
    localStorage.setItem(TOKEN_STORAGE_KEY, token);

    render(<AuthProvider><AuthProbe /></AuthProvider>);

    await waitFor(() => expect(screen.getByTestId('is-authenticated').textContent).toBe('true'));
    expect(screen.getByTestId('user-email').textContent).toBe('admin@adedonha.com');
  });
});
