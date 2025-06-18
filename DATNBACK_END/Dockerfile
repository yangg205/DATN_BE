# Stage 1: Build the application - Giai đoạn 1: Xây dựng ứng dụng
# Sử dụng base image SDK 8.0 từ Microsoft cho quá trình build.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Sao chép tệp project (.csproj) và NuGet.config trước.
# Điều này giúp tận dụng Docker cache hiệu quả hơn,
# vì các dependencies thường ít thay đổi hơn mã nguồn.
COPY ["DATNBACK_END.csproj", "./"]
# Nếu bạn có tệp NuGet.config chứa các nguồn package riêng tư hoặc cấu hình khác,
# hãy đảm bảo nó nằm ở thư mục gốc của build context và được sao chép vào đây.
# Nếu không có, bạn có thể bỏ qua dòng này.
COPY ["NuGet.config", "./"]

# Khôi phục các package NuGet cần thiết cho dự án.
# Lệnh này sẽ tải xuống tất cả các dependencies được khai báo trong .csproj.
RUN dotnet restore "DATNBACK_END.csproj"

# Sao chép toàn bộ mã nguồn còn lại của ứng dụng vào thư mục làm việc.
# Docker sẽ chỉ sao chép những tệp mới hoặc đã thay đổi so với layer trước đó.
COPY . .

# Đặt lại thư mục làm việc là thư mục gốc của project để các lệnh tiếp theo
# có thể tìm thấy mã nguồn chính xác.
WORKDIR "/src"

# Xuất bản ứng dụng ở chế độ Release.
# -c Release: Build ở cấu hình Release.
# -o /app/out: Đặt đầu ra đã xuất bản vào thư mục /app/out trong container.
# --no-restore: Bỏ qua bước khôi phục package vì đã làm ở bước trên.
# /p:UseAppHost=false: Tạo ra một tệp DLL độc lập thay vì một tệp thực thi.
# Điều này phổ biến hơn trong môi trường Docker.
# Thêm /p:DisableImplicitNuGetFallbackFolder=true để ngăn chặn lỗi tìm kiếm thư mục NuGet cục bộ.
RUN dotnet publish -c Release -o /app/out /p:UseAppHost=false /p:DisableImplicitNuGetFallbackFolder=true -v normal

# Stage 2: Create runtime image - Giai đoạn 2: Tạo image chạy ứng dụng
# Sử dụng base image ASP.NET Runtime 8.0, nó nhẹ hơn SDK image và chỉ chứa
# những gì cần thiết để chạy ứng dụng, giúp giảm kích thước image cuối cùng.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Đặt thư mục làm việc mặc định trong container cho ứng dụng.
WORKDIR /app

# Tạo một người dùng không phải root và chuyển quyền sở hữu thư mục /app cho người dùng này.
# Đây là một thực tiễn tốt về bảo mật để ứng dụng không chạy với quyền root.
RUN adduser --system --group appuser && chown -R appuser:appuser /app

# Sao chép các tệp đã được xuất bản (output) từ stage 'build' vào stage 'final'.
# Các tệp này sẽ thuộc sở hữu của 'appuser' sau khi lệnh chown ở trên.
COPY --from=build /app/out .

# Khai báo cổng mà ứng dụng ASP.NET Core của bạn sẽ lắng nghe.
# Mặc dù bạn đã nói cổng mặc định là 7102, lệnh EXPOSE này chỉ mang tính tài liệu
# và thông báo cho người dùng cổng nào nên được sử dụng.
# Ứng dụng thực tế cần được cấu hình để lắng nghe cổng này.
EXPOSE 7102

# Chuyển sang người dùng không phải root trước khi chạy ứng dụng.
USER appuser

# Định nghĩa điểm khởi đầu (ENTRYPOINT) của container.
# Khi container được chạy, lệnh này sẽ được thực thi.
# Nó sẽ chạy ứng dụng .NET của bạn thông qua tệp DLL đã xuất bản.
ENTRYPOINT ["dotnet", "DATNBACK_END.dll"]

# HƯỚNG DẪN SỬ DỤNG:
# 1. Đảm bảo bạn đang ở thư mục gốc của dự án .NET của mình,
#    nơi chứa Dockerfile và tệp .csproj.

# 2. Tạo một tệp `.dockerignore` (nếu chưa có) ở cùng thư mục với Dockerfile.
#    Điều này giúp loại bỏ các tệp không cần thiết (như các tệp tạm, cache, git metadata)
#    khỏi build context, giúp tăng tốc độ build và giảm kích thước image.
#    Nội dung mẫu cho `.dockerignore`:
#    ```
#    bin/
#    obj/
#    .vs/
#    .vscode/
#    *.user
#    *.suo
#    *.pdb
#    *.log
#    node_modules/
#    .git/
#    .gitignore
#    ```

# 3. Build Docker image:
#    Mở Terminal hoặc Command Prompt, điều hướng đến thư mục dự án và chạy:
#    docker build -t datnback_end .
#    (Thay 'datnback_end' bằng tên image bạn muốn)

# 4. Chạy Docker container:
#    Để chạy ứng dụng và ánh xạ cổng 7102 của container ra cổng 7102 của máy chủ,
#    đồng thời cấu hình ứng dụng lắng nghe trên cổng 7102 bên trong container
#    bằng biến môi trường ASPNETCORE_URLS (cách tốt nhất cho Docker):
#    docker run -d -p 7102:7102 -e ASPNETCORE_URLS="http://+:7102" datnback_end
#    (-d để chạy container ở chế độ nền)

#    HOẶC, nếu bạn đã chắc chắn ứng dụng của bạn (trong Program.cs hoặc Startup.cs)
#    đã được cấu hình để lắng nghe "http://*:7102" bằng `.UseUrls("http://*:7102")`:
#    docker run -d -p 7102:7102 datnback_end