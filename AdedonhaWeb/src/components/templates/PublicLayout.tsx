import { Link, Outlet } from 'react-router-dom';
import { AppBar, Box, Button, MenuItem, Select, Toolbar, Typography, type SelectChangeEvent } from '@mui/material';
import { alpha } from '@mui/material/styles';
import { useColorTheme } from '../../contexts/ColorThemeContext';
import { COLOR_THEME_LABELS, type ColorTheme } from '../../types/colorTheme.types';

export const PublicLayout = () => {
  const { colorTheme, setColorTheme } = useColorTheme();

  const handleThemeChange = (event: SelectChangeEvent) => {
    setColorTheme(event.target.value as ColorTheme);
  };

  return (
    <Box>
      <AppBar position="fixed" sx={{ bgcolor: 'chrome.main', color: 'chrome.contrastText' }}>
        <Toolbar sx={{ justifyContent: 'space-between' }}>
          <Typography variant="h6" component="div">Adedonha</Typography>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
            <Button component={Link} to="/sobre" color="inherit">Sobre</Button>
            <Select
              value={colorTheme}
              onChange={handleThemeChange}
              size="small"
              variant="standard"
              sx={{
                color: 'inherit',
                '&:before': { borderColor: (theme) => alpha(theme.palette.chrome.contrastText, 0.4) },
                '& .MuiSvgIcon-root': { color: 'inherit' },
              }}
            >
              {(Object.keys(COLOR_THEME_LABELS) as ColorTheme[]).map((theme) => (
                <MenuItem key={theme} value={theme}>{COLOR_THEME_LABELS[theme]}</MenuItem>
              ))}
            </Select>
          </Box>
        </Toolbar>
      </AppBar>
      <Box component="main" sx={{ pt: 10, px: { xs: 2, md: 4 }, pb: 4 }}>
        <Outlet />
      </Box>
    </Box>
  );
};
