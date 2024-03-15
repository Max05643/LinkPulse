# Link Pulse

It's a simple project that can shorten ("beautify") URLs. It's written using ASP.NET Core 7.0

## Credits

Used projects:
- **Redis**: DB. [redis.io](https://redis.io/)
- **qrcode.js**: generating qr codes on frontend. [davidshimjs.github.io](https://davidshimjs.github.io/qrcodejs/)

## Usage

To deploy this service yourself:

1. Prepare a machine with Docker installed
2. Create a Docker Compose file (e.g., `docker-compose.yml`):

```yaml
version: "3"

services:
    redis:
        image: "redis:latest"
        restart: always
        volumes:
            - redis_links_data:/data
    linkpulse:
        image: "max05643/linkpulse:latest"
        ports:
            - "80:5000"
        depends_on:
            - redis
        environment:
            - ConnectionStrings__redis=redis:6379
            - Shortener__ExpirationTimeSeconds=3600
            - Shortener__ExpandExpirationTimeOnEveryUse=true
volumes:
  redis_links_data:
```

Use `Shortener__ExpirationTimeSeconds` to specify the time in seconds for one shortened url to exist.
Use `Shortener__ExpandExpirationTimeOnEveryUse` to specify whether storage time for the shortened url should be reset on every usage.

4. Run `docker-compose up` in your terminal to start the bot and Redis Server containers.

[![Tests](https://github.com/Max05643/LinkPulse/actions/workflows/tests.yml/badge.svg)](https://github.com/Max05643/LinkPulse/actions/workflows/tests.yml) - Testing workflow status for the master branch.

[![Docker](https://github.com/Max05643/LinkPulse/actions/workflows/docker.yml/badge.svg)](https://github.com/Max05643/LinkPulse/actions/workflows/docker.yml) - Build and push Docker container workflow status.
