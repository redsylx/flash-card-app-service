namespace Main.Consts;

public class EnvironmentVariables : ConstBase {
    public static readonly string Issuer = "JWT_VALID_ISSUER";
    public static readonly string Audience = "JWT_VALID_AUDIENCE";
    public static readonly string SigningKey = "JWT_ISSUER_SIGNING_KEY";
    public static readonly string FrontendUrl = "FE_URL";
}