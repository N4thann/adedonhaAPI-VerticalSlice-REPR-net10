import { useEffect } from 'react';
import { Alert, Box, CircularProgress, Paper, Typography } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import { PieChart } from '@mui/x-charts/PieChart';
import { useAdminDashboardStore } from '../../../store/dashboard/useAdminDashboardStore';
import { DONUT_CHART_COLORS } from '../../../theme';

export const DashboardPage = () => {
  const theme = useTheme();
  const {
    categoryWordCounts, isLoading, error, fetchCategoryWordCounts,
    wordStats, isLoadingWordStats, wordStatsError, fetchWordStats,
  } = useAdminDashboardStore();

  useEffect(() => {
    fetchCategoryWordCounts();
    fetchWordStats();
  }, [fetchCategoryWordCounts, fetchWordStats]);

  return (
    <Box>
      <Typography variant="h4" sx={{ mb: 3 }}>Dashboard</Typography>

      <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
        {isLoading && (
          <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
            <CircularProgress />
          </Box>
        )}
        {error && <Alert severity="error">{error}</Alert>}

        {!isLoading && !error && (
          <Paper sx={{ p: 3, display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
            <Typography variant="h6" sx={{ mb: 1 }}>Categorias por palavra</Typography>
            {wordStats && (
              <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                Total de {wordStats.totalWords} palavra(s) cadastrada(s).
              </Typography>
            )}
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

        {isLoadingWordStats && (
          <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
            <CircularProgress />
          </Box>
        )}
        {wordStatsError && <Alert severity="error">{wordStatsError}</Alert>}

        {!isLoadingWordStats && !wordStatsError && wordStats && (
          <Paper sx={{ p: 3, display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
            <Typography variant="h6" sx={{ mb: 2 }}>Palavras em mais de uma categoria</Typography>
            {wordStats.wordsInMultipleCategories.length === 0 ? (
              <Typography variant="body2" color="text.secondary">Nenhuma palavra está em mais de uma categoria.</Typography>
            ) : (
              <PieChart
                series={[{
                  data: wordStats.wordsInMultipleCategories.map((w) => ({ id: w.slug, value: w.categoryCount, label: w.name })),
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
    </Box>
  );
};
