import { createTheme, type Theme } from '@mui/material/styles';
import type { ColorTheme } from './types/colorTheme.types';

declare module '@mui/material/styles' {
  interface Palette {
    chrome: Palette['primary'];
  }
  interface PaletteOptions {
    chrome?: PaletteOptions['primary'];
  }
}

interface ColorThemePalette {
  mode: 'light' | 'dark';
  primary: string;
  secondary: string;
  background: { default: string; paper: string };
  chrome: string;
}

const COLOR_THEME_PALETTES: Record<ColorTheme, ColorThemePalette> = {
  Blue: {
    mode: 'light',
    primary: '#1E40AF',
    secondary: '#0D9488',
    background: { default: '#F1F5F9', paper: '#FFFFFF' },
    chrome: '#0F172A',
  },
  Red: {
    mode: 'light',
    primary: '#9F1239',
    secondary: '#FB7185',
    background: { default: '#FFF1F2', paper: '#FFFFFF' },
    chrome: '#4C0519',
  },
  Green: {
    mode: 'light',
    primary: '#15803D',
    secondary: '#CA8A04',
    background: { default: '#F0FDF4', paper: '#FFFFFF' },
    chrome: '#052E16',
  },
  Yellow: {
    mode: 'dark',
    primary: '#FEF08A',
    secondary: '#EA580C',
    background: { default: '#171717', paper: '#262626' },
    chrome: '#FEF08A',
  },
  Purple: {
    mode: 'dark',
    primary: '#E9D5FF',
    secondary: '#22D3EE',
    background: { default: '#18181B', paper: '#27272A' },
    chrome: '#E9D5FF',
  },
};

export const DONUT_CHART_COLORS = {
  light: ['#2a78d6', '#eb6834', '#1baf7a', '#eda100', '#e87ba4', '#008300', '#4a3aa7', '#e34948'],
  dark: ['#3987e5', '#d95926', '#199e70', '#c98500', '#d55181', '#008300', '#9085e9', '#e66767'],
} as const;

export function createAppTheme(colorTheme: ColorTheme): Theme {
  const palette = COLOR_THEME_PALETTES[colorTheme];
  const { palette: basePalette } = createTheme({ palette: { mode: palette.mode } });

  return createTheme({
    palette: {
      mode: palette.mode,
      primary: { main: palette.primary },
      secondary: { main: palette.secondary },
      background: palette.background,
      chrome: basePalette.augmentColor({ color: { main: palette.chrome } }),
    },
    shape: { borderRadius: 16 },
  });
}
