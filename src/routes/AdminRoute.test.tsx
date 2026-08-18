import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import { MemoryRouter, Routes, Route } from 'react-router-dom';
import { AdminRoute } from './AdminRoute';
import { useAuth } from '../contexts/AuthContext';

vi.mock('../contexts/AuthContext', () => ({
  useAuth: vi.fn(),
}));

function renderWithRoute() {
  return render(
    <MemoryRouter initialEntries={['/admin/categorias']}>
      <Routes>
        <Route path="/login" element={<div>Tela de login</div>} />
        <Route element={<AdminRoute />}>
          <Route path="/admin/categorias" element={<div>Categorias</div>} />
        </Route>
      </Routes>
    </MemoryRouter>
  );
}

describe('AdminRoute', () => {
  it('redireciona para /login quando não há usuário autenticado', () => {
    vi.mocked(useAuth).mockReturnValue({ user: null } as ReturnType<typeof useAuth>);
    renderWithRoute();
    expect(screen.getByText('Tela de login')).toBeInTheDocument();
  });

  it('redireciona para /login quando o usuário não é admin', () => {
    vi.mocked(useAuth).mockReturnValue({ user: { isAdmin: false } } as ReturnType<typeof useAuth>);
    renderWithRoute();
    expect(screen.getByText('Tela de login')).toBeInTheDocument();
  });

  it('renderiza a rota filha quando o usuário é admin', () => {
    vi.mocked(useAuth).mockReturnValue({ user: { isAdmin: true } } as ReturnType<typeof useAuth>);
    renderWithRoute();
    expect(screen.getByText('Categorias')).toBeInTheDocument();
  });
});
