import { describe, it, expect, beforeEach } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import { ColorThemeProvider, useColorTheme } from './ColorThemeContext';
import { COLOR_THEME_STORAGE_KEY } from '../types/colorTheme.types';

function ColorThemeProbe() {
  const { colorTheme, setColorTheme } = useColorTheme();
  return (
    <div>
      <span data-testid="color-theme-value">{colorTheme}</span>
      <button onClick={() => setColorTheme('Green')}>trocar</button>
    </div>
  );
}

describe('ColorThemeContext', () => {
  beforeEach(() => {
    localStorage.clear();
  });

  it('usa Blue como padrão quando nada está salvo', () => {
    render(<ColorThemeProvider><ColorThemeProbe /></ColorThemeProvider>);
    expect(screen.getByTestId('color-theme-value').textContent).toBe('Blue');
  });

  it('lê o tema previamente salvo no localStorage', () => {
    localStorage.setItem(COLOR_THEME_STORAGE_KEY, 'Purple');
    render(<ColorThemeProvider><ColorThemeProbe /></ColorThemeProvider>);
    expect(screen.getByTestId('color-theme-value').textContent).toBe('Purple');
  });

  it('ignora um valor inválido salvo e cai no padrão', () => {
    localStorage.setItem(COLOR_THEME_STORAGE_KEY, 'NaoExiste');
    render(<ColorThemeProvider><ColorThemeProbe /></ColorThemeProvider>);
    expect(screen.getByTestId('color-theme-value').textContent).toBe('Blue');
  });

  it('persiste o novo tema ao trocar', () => {
    render(<ColorThemeProvider><ColorThemeProbe /></ColorThemeProvider>);
    fireEvent.click(screen.getByText('trocar'));

    expect(screen.getByTestId('color-theme-value').textContent).toBe('Green');
    expect(localStorage.getItem(COLOR_THEME_STORAGE_KEY)).toBe('Green');
  });
});
