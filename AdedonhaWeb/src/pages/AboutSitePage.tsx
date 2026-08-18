import { useEffect } from 'react';
import { Alert, Avatar, Box, Card, CardContent, Chip, CircularProgress, List, ListItem, ListItemText, Typography } from '@mui/material';
import { useAboutSiteStore } from '../store/aboutSite/useAboutSiteStore';
import { resolveUploadUrl } from '../services/api';

export const AboutSitePage = () => {
  const { data, isLoading, error, fetchAboutSite } = useAboutSiteStore();

  useEffect(() => {
    fetchAboutSite();
  }, [fetchAboutSite]);

  const hasContent =
    !!data &&
    (!!data.cargo || !!data.textoGeral || data.formacoes.length > 0 || data.tecnologias.length > 0 || data.arquiteturas.length > 0);

  return (
    <Box>
      <Typography variant="h4" sx={{ mb: 3 }}>Sobre o site</Typography>

      {isLoading && (
        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
          <CircularProgress />
        </Box>
      )}
      {error && <Alert severity="error">{error}</Alert>}

      {!isLoading && !error && (
        <Card>
          <CardContent sx={{ py: 4 }}>
            {hasContent ? (
              <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: '280px 1fr 280px' }, gap: 4 }}>
                <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 2 }}>
                  <Avatar src={resolveUploadUrl(data?.imageUrl ?? null) ?? undefined} sx={{ width: 120, height: 120 }} />
                  <Typography variant="h6" sx={{ fontWeight: 'bold' }}>{data?.cargo}</Typography>

                  {data && data.formacoes.length > 0 && (
                    <List dense sx={{ width: '100%' }}>
                      {data.formacoes.map((formacao, index) => (
                        <ListItem key={index} sx={{ justifyContent: 'center' }}>
                          <ListItemText primary={formacao} sx={{ textAlign: 'center', flexGrow: 0 }} />
                        </ListItem>
                      ))}
                    </List>
                  )}
                </Box>

                <Box>
                  <Typography variant="h6" sx={{ fontWeight: 'bold', mb: 2 }}>Sobre</Typography>
                  <Typography variant="body1">{data?.textoGeral}</Typography>
                </Box>

                <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
                  {data && data.tecnologias.length > 0 && (
                    <Box>
                      <Typography variant="h6" sx={{ fontWeight: 'bold', mb: 1 }}>Tecnologias e ferramentas</Typography>
                      <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                        {data.tecnologias.map((tecnologia, index) => <Chip key={index} label={tecnologia} />)}
                      </Box>
                    </Box>
                  )}

                  {data && data.arquiteturas.length > 0 && (
                    <Box>
                      <Typography variant="h6" sx={{ fontWeight: 'bold', mb: 1 }}>Arquiteturas estudadas</Typography>
                      <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                        {data.arquiteturas.map((arquitetura, index) => <Chip key={index} label={arquitetura} />)}
                      </Box>
                    </Box>
                  )}
                </Box>
              </Box>
            ) : (
              <Typography variant="body1" color="text.secondary" sx={{ textAlign: 'center', fontStyle: 'italic' }}>
                Nenhum conteúdo cadastrado ainda.
              </Typography>
            )}
          </CardContent>
        </Card>
      )}
    </Box>
  );
};
