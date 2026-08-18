import { useMemo, type ReactNode } from 'react';
import { BrowserRouter } from 'react-router-dom';
import { CssBaseline, ThemeProvider } from '@mui/material';
import { ColorThemeProvider, useColorTheme } from './contexts/ColorThemeContext';
import { AuthProvider } from './contexts/AuthContext';
import { createAppTheme } from './theme';
import { AppRoutes } from './routes';

function AppThemeProvider({ children }: { children: ReactNode }) {
  const { colorTheme } = useColorTheme();
  const theme = useMemo(() => createAppTheme(colorTheme), [colorTheme]);

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      {children}
    </ThemeProvider>
  );
}

function App() {
  return (
    <ColorThemeProvider>
      <AppThemeProvider>
        <AuthProvider>
          <BrowserRouter>
            <AppRoutes />
          </BrowserRouter>
        </AuthProvider>
      </AppThemeProvider>
    </ColorThemeProvider>
  );
}

export default App;
