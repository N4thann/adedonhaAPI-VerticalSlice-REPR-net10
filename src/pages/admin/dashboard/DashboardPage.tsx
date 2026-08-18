import { useEffect } from 'react';
import { Alert, Box, CircularProgress, Paper, Typography } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import { PieChart } from '@mui/x-charts/PieChart';
import { useAdminDashboardStore } from '../../../store/dashboard/useAdminDashboardStore';
import { DONUT_CHART_COLORS } from '../../../theme';

export const DashboardPage = () => {
  const theme = useTheme();
  const { categoryWordCounts, isLoading, error, fetchCategoryWordCounts } = useAdminDashboardStore();

  useEffect(() => {
    fetchCategoryWordCounts();
  }, [fetchCategoryWordCounts]);

  return (
    <Box>
      <Typography variant="h4" sx={{ mb: 3 }}>Dashboard</Typography>

      {isLoading && (
        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
          <CircularProgress />
        </Box>
      )}
      {error && <Alert severity="error">{error}</Alert>}

      {!isLoading && !error && (
        <Paper sx={{ p: 3, display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
          <Typography variant="h6" sx={{ mb: 2 }}>Categorias por palavra</Typography>
          {categoryWordCounts.length === 0 ? (
            <Typography variant="body2" color="text.secondary">Nenhum dado disponível ainda.</Typography>
          ) : (
            <PieChart
              series={[{
                data: categoryWordCounts.map((c) => ({ id: c.slug, value: c.wordCount, label: c.name })),
                innerRadius: 60,
                outerRadius: 120,
              }]}
              colors={DONUT_CHART_COLORS[theme.palette.mode]}
              width={400}
              height={300}
            />
          )}
        </Paper>
      )}
    </Box>
  );
};
