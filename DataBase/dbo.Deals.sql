CREATE TABLE [dbo].[Deals] (
    [id]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [dealDate]         DATE           NULL,
    [buyerInn]         NVARCHAR (MAX) DEFAULT (NULL) NULL,
    [buyerName]        NVARCHAR (MAX) NULL,
    [sellerInn]        NVARCHAR (MAX) NULL,
    [sellerName]       NVARCHAR (MAX) DEFAULT (NULL) NULL,
    [woodVolumeBuyer]  FLOAT (53)     NOT NULL,
    [woodVolumeSeller] FLOAT (53)     NOT NULL,
    [dealNumber]       NVARCHAR (30)  NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);


GO


CREATE INDEX [IX_Deals_find] ON [dbo].[Deals] ([dealNumber],[dealDate])
