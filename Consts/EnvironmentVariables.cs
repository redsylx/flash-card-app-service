namespace Main.Consts;

public class EnvironmentVariables : ConstBase {
    public static readonly string Issuer = "JWT_VALID_ISSUER";
    public static readonly string Audience = "JWT_VALID_AUDIENCE";
    public static readonly string SigningKey = "JWT_ISSUER_SIGNING_KEY";
    public static readonly string FrontendUrl = "FE_URL";
    public static readonly string STORAGE_ACCOUNT_NAME = "STORAGE_ACCOUNT_NAME";
    public static readonly string STORAGE_ACCOUNT_KEY = "STORAGE_ACCOUNT_KEY";
    public static readonly string CONTAINER_NAME_IMAGE = "STORAGE_IMAGE_CONTAINER";
}