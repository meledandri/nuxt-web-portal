SELECT        NULL AS id, 2 AS sid, Tbl_FolderTable_ECTD.title, Tbl_FolderTable_ECTD.IDparent AS parentID, Tbl_FolderTable_ECTD.IDfoldertable AS idDocumento, iif(Tbl_CTD.Elemento is null, Tbl_FolderTable_ECTD.title, Tbl_CTD.Elemento) AS fileName, Tbl_FolderTable_ECTD.AddFolder, 
                         Tbl_FolderTable_ECTD.AddFile, Tbl_FolderTable_ECTD.NLivelli, 0 AS idVerDoc, 0 AS flagState, '' AS fileExtension, 0 AS operatorID, '' AS MD5, 1 AS swTarget, '' AS file_for_checklist, iif(Tbl_CTD.Path is null, '', Tbl_CTD.Path) AS fullPath, 
                         Tbl_FolderTable_ECTD.flag_contenitore
FROM            Tbl_CTD RIGHT OUTER JOIN
                         Tbl_FolderTable_ECTD ON Tbl_CTD.Number = Tbl_FolderTable_ECTD.number AND Tbl_CTD.FlagContenitore = Tbl_FolderTable_ECTD.flag_contenitore
WHERE        (Tbl_FolderTable_ECTD.flag_visibile = 0) AND (Tbl_FolderTable_ECTD.ripetizione <> 'r')
ORDER BY idDocumento