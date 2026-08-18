export interface AboutSite {
  cargo: string;
  formacoes: string[];
  textoGeral: string;
  tecnologias: string[];
  arquiteturas: string[];
  imageUrl: string | null;
}

export interface AboutSiteUpsertPayload {
  cargo: string;
  formacoes: string[];
  textoGeral: string;
  tecnologias: string[];
  arquiteturas: string[];
  image?: File;
}
