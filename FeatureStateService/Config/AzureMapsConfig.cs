namespace FeatureStateService.Config;

public record class AzureMapsConfig(
    string Domain,
    string ClientId,
    string TokenUrl,
    string DatasetId,
    string MapConfigurationId,
    string APIVersion,
    string SourceLayer);
