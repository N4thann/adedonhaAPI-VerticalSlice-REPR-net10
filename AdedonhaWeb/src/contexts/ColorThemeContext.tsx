import { createContext, useContext, useState, type ReactNode } from 'react';
import { COLOR_THEME_STORAGE_KEY, isColorTheme, type ColorTheme } from '../types/colorTheme.types';

const DEFAULT_COLOR_THEME: ColorTheme = 'Blue';

function readStoredColorTheme(): ColorTheme {
  const stored = localStorage.getItem(COLOR_THEME_STORAGE_KEY);
  return isColorTheme(stored) ? stored : DEFAULT_COLOR_THEME;
}

interface ColorThemeContextType {
  colorTheme: ColorTheme;
  setColorTheme: (theme: ColorTheme) => void;
}

const ColorThemeContext = createContext<ColorThemeContextType>({} as ColorThemeContextType);

export const ColorThemeProvider = ({ children }: { children: ReactNode }) => {
  const [colorTheme, setColorThemeState] = useState<ColorTheme>(readStoredColorTheme);

  const setColorTheme = (theme: ColorTheme) => {
    localStorage.setItem(COLOR_THEME_STORAGE_KEY, theme);
    setColorThemeState(theme);
  };

  return (
    <ColorThemeContext.Provider value={{ colorTheme, setColorTheme }}>
      {children}
    </ColorThemeContext.Provider>
  );
};

// eslint-disable-next-line react-refresh/only-export-components -- hook colocated with its Provider, standard Context pattern
export const useColorTheme = () => useContext(ColorThemeContext);
